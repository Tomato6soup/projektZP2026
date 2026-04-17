using System;
using System.Drawing;
using System.Windows.Forms;

namespace BazaPublikacji_app
{
    public partial class MainForm
    {
       
        // PrzelaczTeme, UstawTeme, UpdateCardsColors, WyswietlPowitanie
        private void WyswietlPowitanie()
        {
            lblPowitanie.Text = $"Witaj, {zalogowanyUzytkownik}!";
        }

        private void PrzelaczTeme()
        {
            // Cycle through available themes
            var values = Enum.GetValues(typeof(AppTheme));
            int next = ((int)currentTheme + 1) % values.Length;
            currentTheme = (AppTheme)values.GetValue(next);
            darkTheme = currentTheme == AppTheme.Dark; // keep backward compatibility
            UpdateBtnThemeIcon();
            UstawTeme();
        }

        private void UpdateBtnThemeIcon()
        {
            if (btnTemat == null) return;

            // Simple visual indicator for current theme
            switch (currentTheme)
            {
                case AppTheme.Dark:
                    btnTemat.Text = "🌙";
                    break;
                case AppTheme.Light:
                    btnTemat.Text = "☀️";
                    break;
                case AppTheme.Pink:
                    btnTemat.Text = "🌸";
                    break;
                case AppTheme.Green:
                    btnTemat.Text = "🌿";
                    break;
                case AppTheme.Blue:
                    btnTemat.Text = "🌊";
                    break;
                default:
                    btnTemat.Text = "🎨";
                    break;
            }
        }

        private void UstawTeme()
        {
            // Default values (will be overridden per theme)
            Color backMain = Color.White;
            Color backTop = Color.LightGray;
            Color backSide = Color.LightGray;
            Color backCard = Color.White;
            Color foreColor = Color.Black;
            Color hoverCard = Color.FromArgb(240, 240, 255);
            Color btnCardColor = Color.White;

            // Theme-specific colors
            switch (currentTheme)
            {
                case AppTheme.Dark:
                    backMain = Color.FromArgb(40, 40, 60);
                    backTop = Color.FromArgb(60, 60, 90);
                    backSide = Color.FromArgb(30, 30, 50);
                    backCard = Color.FromArgb(50, 50, 70);
                    foreColor = Color.White;
                    hoverCard = Color.FromArgb(180, 150, 255);
                    btnCardColor = Color.FromArgb(70, 70, 90);
                    break;

                case AppTheme.Light:
                    backMain = Color.White;
                    backTop = Color.LightGray;
                    backSide = Color.LightGray;
                    backCard = Color.White;
                    foreColor = Color.Black;
                    hoverCard = Color.FromArgb(240, 240, 255);
                    btnCardColor = Color.White;
                    break;

                case AppTheme.Pink:
                    backMain = Color.FromArgb(255, 240, 245);
                    backTop = Color.FromArgb(255, 225, 235);
                    backSide = Color.FromArgb(255, 210, 225);
                    backCard = Color.FromArgb(255, 245, 250);
                    foreColor = Color.FromArgb(40, 20, 40);
                    hoverCard = Color.FromArgb(255, 200, 230);
                    btnCardColor = Color.FromArgb(255, 230, 240);
                    break;

                case AppTheme.Green:
                    backMain = Color.FromArgb(235, 255, 235);
                    backTop = Color.FromArgb(220, 245, 220);
                    backSide = Color.FromArgb(200, 235, 200);
                    backCard = Color.FromArgb(240, 255, 240);
                    foreColor = Color.FromArgb(10, 60, 20);
                    hoverCard = Color.FromArgb(200, 255, 200);
                    btnCardColor = Color.FromArgb(220, 245, 220);
                    break;

                case AppTheme.Blue:
                    backMain = Color.FromArgb(235, 245, 255);
                    backTop = Color.FromArgb(215, 230, 255);
                    backSide = Color.FromArgb(200, 220, 245);
                    backCard = Color.FromArgb(240, 250, 255);
                    foreColor = Color.FromArgb(10, 30, 70);
                    hoverCard = Color.FromArgb(200, 225, 255);
                    btnCardColor = Color.FromArgb(220, 235, 255);
                    break;
            }

            // Apply colors
            this.BackColor = backMain;
            panelGorny.BackColor = backTop;
            panelBoczny.BackColor = backSide;

            foreach (Control c in panelGorny.Controls)
            {
                c.BackColor = darkTheme ? Color.FromArgb(80, 80, 100) : Color.White;
                c.ForeColor = foreColor;
            }

            foreach (Control c in panelBoczny.Controls)
            {
                if (c is Button btn)
                {
                    btn.BackColor = backSide;
                    btn.ForeColor = foreColor;
                }
            }

            // Panels with cards (null-check to avoid NRE during initialization)
            UpdateCardsColors(pnlPublikacje, backCard, btnCardColor, foreColor, hoverCard);
            UpdateCardsColors(pnlProjekty, backCard, btnCardColor, foreColor, hoverCard);
            UpdateCardsColors(pnlKonferencje, backCard, btnCardColor, foreColor, hoverCard);
            UpdateCardsColors(pnlUlubione, backCard, btnCardColor, foreColor, hoverCard);

            foreach (TabPage tab in zakladki.TabPages)
            {
                tab.BackColor = backMain;
                foreach (Control c in tab.Controls)
                    if (c is Label lbl) lbl.ForeColor = foreColor;
            }
        }

        private void UpdateCardsColors(FlowLayoutPanel panel, Color backCard, Color btnCardColor, Color foreColor, Color hoverCard)
        {
            if (panel == null) return;

            foreach (Control c in panel.Controls)
            {
                if (c is Panel card)
                {
                    // store colors so class-level handlers can read them
                    card.Tag = Tuple.Create(backCard, hoverCard);

                    card.BackColor = backCard;

                    // detach class-level handlers first (safe even if not attached)
                    card.MouseEnter -= Card_MouseEnter;
                    card.MouseLeave -= Card_MouseLeave;

                    // attach class-level handlers
                    card.MouseEnter += Card_MouseEnter;
                    card.MouseLeave += Card_MouseLeave;

                    foreach (Control child in card.Controls)
                    {
                        if (child is Button btn)
                        {
                            btn.BackColor = btnCardColor;
                            btn.ForeColor = foreColor;
                        }
                        else if (child != null)
                        {
                            child.ForeColor = foreColor;
                        }
                    }
                }
            }
        }

        // class-level handlers use Tag to obtain colors (so removal works correctly)
        private void Card_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Panel card && card.Tag is Tuple<Color, Color> tup)
            {
                card.BackColor = tup.Item2; // hover color
            }
        }

        private void Card_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Panel card && card.Tag is Tuple<Color, Color> tup)
            {
                card.BackColor = tup.Item1; // original back color
            }
        }
    }
}
