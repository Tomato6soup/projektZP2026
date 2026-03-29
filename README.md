# Info o projekcie: ResearchHub

<img width="1408" alt="Project Header" src="https://github.com/user-attachments/assets/51237d2e-22db-4eb8-8b74-bcbc157c8dc4" />

## Opis projektu
_Projekt to **aplikacja desktopowa** stworzona w technologii **C#** z wykorzystaniem **WPF** oraz wzorca **MVVM**. Służy do zarządzania i przeglądania publikacji naukowych oraz projektów._

_Aplikacja korzysta z bazy danych **SQL Server**, а rozwój projektu był wspierany przez system kontroli wersji **GitHub**._

---

## Co aplikacja robi?
Aplikacja oferuje szeroki zakres funkcji dla różnych typów użytkowników:

* **Autoryzacja:** Logowanie do systemu z rolami: *Student, Pracownik, Administrator*.
* **Przeglądanie:** Dostęp do szczegółowych informacji o projektach i publikacjach.
* **Wyszukiwanie:** Zaawansowane filtrowanie danych według fraz, roku wydania czy wydawnictwa.
* **Ulubione:** Możliwość dodawania publikacji do listy ulubionych na stronie głównej.
* **Zarządzanie (Admin):** Dodawanie nowych publikacji i edycja bazy danych.
* **Konferencje:** Informacje o nadchodzących wydarzeniach naukowych.
* **Personalizacja:** Przełączanie między **trybem jasnym a ciemnym** 🌙.

---

## Dla kogo jest aplikacja?
<img width="1408" alt="Target Audience" src="https://github.com/user-attachments/assets/18beadd2-9be4-4e0c-92de-ad0103c1e623" />

- [x] **Studenci** – przeglądanie projektów i publikacji.
- [x] **Pracownicy naukowi** – szybki dostęp do zasobów wiedzy.
- [x] **Administratorzy** – pełne zarządzanie systemem.

---

## Wymagania

### Funkcjonalne:
1. Logowanie i automatyczne rozpoznawanie ról.
2. Wyszukiwanie i filtrowanie publikacji.
3. Przeglądanie szczegółów projektów и konferencji.
4. Zarządzanie bazą danych (dla Admina).
5. Zmiana motywu graficznego (Light/Dark mode).

### Niefunkcjonalne:
- **Wydajność:** Szybkie przetwarzanie zapytań SQL.
- **Bezpieczeństwo:** Autoryzacja użytkowników.
- **Użyteczność:** Intuicyjny interfejs WPF.
- **Niezawodność:** Stabilne połączenie z SQL Server.

---

## Przypadki użycia
<img width="954" alt="Use Cases Diagram" src="https://github.com/user-attachments/assets/179a1ca9-6ac0-4078-8cda-2222ee730a03" />


### 1. Autoryzacja i Logowanie
*Opis procesu logowania w zależności od uprawnień dostępu.*
1. **Inicjacja:** Użytkownik uruchamia aplikację i wprowadza dane uwierzytelniające (Login, Hasło).
2. **Weryfikacja:** System przesyła zapytanie do bazy danych SQL Server w celu sprawdzenia poprawności danych.
3. **Autoryzacja:** Po pomyślnej weryfikacji system rozpoznaje przypisaną rolę (np. *Administrator*).
4. **Finał:** Użytkownik otrzymuje dostęp do panelu głównego z funkcjami odpowiadającymi jego uprawnieniom.

---

### 2. Wyszukiwanie i Filtrowanie Publikacji
*Proces odnalezienia konkretnej wiedzy w zasobach systemu.*
1. **Nawigacja:** Użytkownik przechodzi do modułu „Publikacje”.
2. **Kryteria:** Wprowadza tytuł w polu wyszukiwania lub wybiera filtry (np. Rok wydania: 2023, Typ: Artykuł).
3. **Przetwarzanie:** ViewModel wywołuje logikę filtrowania z Modelu, która komunikuje się z bazą danych.
4. **Prezentacja:** Wyniki są dynamicznie wyświetlane w widoku (DataGrid) z możliwością podglądu szczegółów.

---

### 3. Zarządzanie Bazą (Tylko Administrator)
*Scenariusz dodawania nowych zasobów naukowych.*
1. **Dostęp:** Administrator wybiera opcję „Dodaj nową publikację”.
2. **Formularz:** Wprowadza wymagane metadane (Tytuł, Autor, Wydawnictwo, plik PDF).
3. **Zatwierdzenie:** Po kliknięciu przycisku „Zapisz”, system waliduje poprawność danych.
4. **Aktualizacja:** Nowy rekord zostaje trwale zapisany w tabeli `Publikacja`, a lista w aplikacji odświeża się automatycznie.
---

## Model danych
<img width="1408" alt="Data Model" src="https://github.com/user-attachments/assets/71725402-ddab-40e1-9987-570e79f9f60f" />

<details>
<summary><b>Kliknij, aby zobaczyć strukturę encji</b></summary>

- **Użytkownik:** `ID, Login, Hasło, Email, Rola, DataRejestracji`
- **Publikacja:** `ID, Tytuł, Rok_Wydania, Typ, Wydawnictwo, PlikPDF, Strony`
- **Projekt:** `ID, Tytuł, Opis, DataRozpoczecia, DataZakonczenia`
- **Pracownik:** `ID, Imię, Nazwisko, Stanowisko, Wydział, Adres, E-mail`
- **Strona:** `ID, Nazwa, Typ, Kraj`
- **Konferencja:** `ID, Nazwa, Data, Miejsce`
- **Wydział:** `ID, Nazwa`
</details>

---

## Architektura systemu
<img width="1408" alt="System Architecture" src="https://github.com/user-attachments/assets/e1ade36e-b32f-44cf-8050-126c63c0215c" />

Aplikacja działa w oparciu o przepływ:
**Użytkownik** ➔ **WPF (View)** ➔ **ViewModel** ➔ **Model** ➔ **SQL Server**

**Aplikacja została zaprojektowana w architekturze:**
- Frontend (View) – interfejs użytkownika w WPF;
- ViewModel – logika prezentacji (MVVM);
- Model – dane i logika biznesowa;
- Baza danych – SQL Server;
---

## Technologie
* **C#** – Główny język programowania.
* **WPF** – Nowoczesny interfejs użytkownika.
* **MVVM** – Separacja logiki od widoku.
* **SQL Server** – Wydajna baza danych.
* **GitHub** – Kontrola wersji.

---

## Autorzy
<img width="1408" alt="Authors" src="https://github.com/user-attachments/assets/95e9d35e-f846-417d-ab10-7319435f4538" />

### Projekt został zrealizowany z pasją przez zespół deweloperski в składzie:

* **Anna Tkach** —   *Lead Developer / Database Architect* 
* **Polina Lysohor** — *UI/UX Designer / WPF Specialist*
* **Iryna Hrysheta** —*Backend Developer / MVVM Logic* 

---
> [!NOTE]
> _Wykonano w ramach projektu akademickiego 2024._
---
