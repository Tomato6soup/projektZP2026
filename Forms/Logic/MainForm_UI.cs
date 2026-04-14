using System;
using System.Drawing;
using System.Windows.Forms;

namespace BazaPublikacji_app
{
    public partial class MainForm
    {
        // Переносим методы: PrzelaczTeme, UstawTeme, UpdateCardsColors, WyswietlPowitanie
        private void WyswietlPowitanie()
        {
            lblPowitanie.Text = $"Witaj, {zalogowanyUzytkownik}!";
        }

        private void PrzelaczTeme()
        {
            darkTheme = !darkTheme;
            UstawTeme();
        }

        private void UstawTeme()
        {
            Color backMain = darkTheme ? Color.FromArgb(40, 40, 60) : Color.White;
            Color backTop = darkTheme ? Color.FromArgb(60, 60, 90) : Color.LightGray;
            Color backSide = darkTheme ? Color.FromArgb(30, 30, 50) : Color.LightGray;
            Color backCard = darkTheme ? Color.FromArgb(50, 50, 70) : Color.White;
            Color foreColor = darkTheme ? Color.White : Color.Black;
            Color hoverCard = darkTheme ? Color.FromArgb(180, 150, 255) : Color.FromArgb(240, 240, 255);
            Color btnCardColor = darkTheme ? Color.FromArgb(70, 70, 90) : Color.White;

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
            foreach (Control c in panel.Controls)
            {
                if (c is Panel card)
                {
                    card.BackColor = backCard;
                    foreach (Control child in card.Controls)
                    {
                        if (child is Button btn)
                        {
                            btn.BackColor = btnCardColor;
                            btn.ForeColor = foreColor;
                        }
                        else child.ForeColor = foreColor;
                    }
                }
            }
        }
    }
}