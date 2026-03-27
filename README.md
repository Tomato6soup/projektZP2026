# Info o projekcie

<img width="1408" height="768" alt="Gemini_Generated_Image_qy6ajgqy6ajgqy6a" src="https://github.com/user-attachments/assets/51237d2e-22db-4eb8-8b74-bcbc157c8dc4" />

## Opis projektu

_Projekt to aplikacja desktopowa stworzona w technologii C# z wykorzystaniem WPF oraz wzorca MVVM, służąca do zarządzania i przeglądania publikacji naukowych oraz projektów. System umożliwia użytkownikom wyszukiwanie, filtrowanie oraz przeglądanie informacji o pracach naukowych i projektach, a administratorowi dodatkowo zarządzanie danymi._

_Aplikacja korzysta z bazy danych SQL Server, zawierającej przykładowe dane, a rozwój projektu był wspierany przez system kontroli wersji GitHub._

**Co aplikacja robi**

_Aplikacja umożliwia:_
1. Aplikacja umożliwia użytkownikom logowanie się do systemu oraz korzystanie z dostępnych funkcji w zależności od przypisanej roli (Student, Pracownik lub Administrator).
2. Użytkownik po zalogowaniu ma możliwość przeglądania projektów naukowych oraz publikacji, wraz z ich szczegółowymi informacjami, takimi jak tytuł, rok wydania, typ czy wydawnictwo.
3. System umożliwia wyszukiwanie publikacji na podstawie wprowadzonych fraz, a także ich filtrowanie według różnych kryteriów, co pozwala szybko znaleźć potrzebne informacje.
4. Użytkownik może również dodawać wybrane publikacje do listy ulubionych, która jest wyświetlana na stronie głównej aplikacji.
5. Administrator posiada rozszerzone uprawnienia, dzięki którym może zarządzać danymi w systemie, w szczególności dodawać nowe publikacje do bazy danych.
6. Aplikacja zapewnia także możliwość przeglądania konferencji oraz zapoznawania się z ich szczegółami, takimi jak nazwa, data i miejsce wydarzenia.
7. System oferuje funkcję wyświetlania szczegółowych informacji o wybranych elementach (publikacjach, projektach, konferencjach) w osobnym panelu.
8. Użytkownik ma możliwość personalizacji wyglądu aplikacji poprzez przełączanie się między trybem jasnym a ciemnym, co zwiększa komfort pracy.
9. Dodatkowo aplikacja posiada intuicyjny interfejs graficzny z podziałem na sekcje (zakładki), co ułatwia nawigację i organizację danych.

**Dla kogo jest aplikacja**

<img width="1408" height="768" alt="Gemini_Generated_Image_jzik8jzik8jzik8j" src="https://github.com/user-attachments/assets/18beadd2-9be4-4e0c-92de-ad0103c1e623" />

_System jest przeznaczony dla:_

 Studentów – przeglądanie projektów i publikacji;
 
 Pracowników naukowych – dostęp do informacji o projektach i publikacjach;
 
 Administratorów – zarządzanie publikacjami i systemem;

**Wymagania funkcjonalne**

1. System umożliwia użytkownikowi zalogowanie się do aplikacji.
2. Po zalogowaniu system automatycznie rozpoznaje rolę użytkownika (Student, Pracownik lub Administrator).
3. Użytkownik ma możliwość wyszukiwania publikacji na podstawie ich tytułu.
4. Aplikacja pozwala filtrować publikacje według wybranych kryteriów, takich jak rok wydania, typ czy wydawnictwo.
5. Użytkownik może przeglądać dostępne projekty oraz zapoznawać się z ich szczegółowymi informacjami.
6. System wyświetla ulubione publikacje użytkownika na stronie głównej.
7. Administrator ma możliwość dodawania nowych publikacji do bazy danych.
8. Użytkownik może zmieniać wygląd aplikacji, przełączając się między trybem jasnym a ciemnym. 

**Wymagania niefunkcjonalne**

_Wydajność_ – szybkie wyszukiwanie i filtrowanie danych.
_Bezpieczeństwo_ – autoryzacja użytkowników i podział ról.
_Użyteczność_ – intuicyjny interfejs graficzny (WPF).
_Niezawodność_ – stabilne połączenie z bazą danych.
_Skalowalność_ – możliwość rozbudowy o nowe funkcjonalności.
_Dostępność_ – wsparcie dla trybu jasnego i ciemnego.
_Spójność danych_ – wykorzystanie SQL Server do zarządzania danymi.

## Przypadki użycia
<img width="954" height="632" alt="plan_uzycia_final drawio" src="https://github.com/user-attachments/assets/179a1ca9-6ac0-4078-8cda-2222ee730a03" />

**1. Logowanie użytkownika**

Użytkownik uruchamia aplikację

Wprowadza login i hasło

System weryfikuje dane

Użytkownik zostaje zalogowany i widzi widok dostosowany do swojej roli

**2. Wyszukiwanie publikacji**

Użytkownik przechodzi do zakładki „Publikacje”

Wpisuje tytuł w wyszukiwarce

System wyświetla pasujące wyniki


**3. Dodawanie publikacji (Administrator)**

Administrator przechodzi do sekcji publikacji

Wybiera opcję „Dodaj publikację”

Wprowadza dane

System zapisuje nową publikację w bazie danych

## Model danych

<img width="1408" height="768" alt="Gemini_Generated_Image_87942i87942i8794(1)" src="https://github.com/user-attachments/assets/71725402-ddab-40e1-9987-570e79f9f60f" />

System przechowuje dane w postaci następujących encji:

_Użytkownik_ (ID, Login, Hasło, Email, Rola, DataRejestracji);

_Publikacja_ (ID, Tytuł, Rok_Wydania, Typ, Wydawnictwo, PlikPDF, Strony);

_Projekt_ (ID, Tytuł, Opis, DataRozpoczecia, DataZakonczenia);

_Pracownik_ (ID, Imię, Nazwisko, Stanowisko, Wydział, Adres, E-mail);

_Strona_(ID, Nazwa, Typ, Kraj);

_Konferencja_ (ID, Nazwa, Data, Miejsce);

_Wydział_ (ID, Nazwa);


## Architektura systemu
<img width="1408" height="768" alt="Gemini_Generated_Image_2ubyhd2ubyhd2uby" src="https://github.com/user-attachments/assets/e1ade36e-b32f-44cf-8050-126c63c0215c" />

**Aplikacja została zaprojektowana w architekturze:**

Frontend (View) – interfejs użytkownika w WPF;

ViewModel – logika prezentacji (MVVM);

Model – dane i logika biznesowa;

Baza danych – SQL Server;

**Schemat:**

Użytkownik → WPF (View) → ViewModel → Model → SQL Server


## Technologie + uzasadnienie

C# – główny język programowania, silne wsparcie dla aplikacji desktopowych;

WPF (Windows Presentation Foundation) – tworzenie nowoczesnego interfejsu użytkownika;

MVVM – separacja logiki od widoku, lepsza testowalność;

SQL Server – wydajna i stabilna baza danych;

SSMS (SQL Server Management Studio) – zarządzanie bazą danych;

GitHub – kontrola wersji i współpraca zespołowa;

Visual Studio – środowisko programistyczne.


# Autorzy:
<img width="1408" height="768" alt="Gemini_Generated_Image_jjjyeajjjyeajjjy" src="https://github.com/user-attachments/assets/95e9d35e-f846-417d-ab10-7319435f4538" />

## _Polina Lysohor, Iryna Hrysheta, Anna Tkach._
