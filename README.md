# TrackApp

TrackApp to aplikacja do zarządzania wydarzeniami, zbudowana jako SPA z frontendem w **Vue.js** i backendem REST API. Projekt jest podzielony na moduły odpowiedzialności, co ułatwia rozwój i utrzymanie kodu.

## Struktura frontendu (`TrackVue/client/src`)

- **Components** – małe, reużywalne elementy interfejsu (np. nagłówki, stopki, formularze).  
- **Views** – główne widoki aplikacji powiązane z routingiem (`HomeView.vue`, `LoginView.vue`, `AddEvent.vue`, `MyEvents.vue`, `PendingEvents.vue`).  
- **Router** – konfiguracja ścieżek URL dla widoków (`index.js`).  
- **Assets** – statyczne zasoby: grafiki, czcionki, style globalne.  
- **App.vue** – główny komponent root, zawierający pasek nawigacji i `<router-view>`.  
- **main.js** – punkt wejścia aplikacji, inicjalizacja Vue i globalnych bibliotek.  
- **axios.js** – konfiguracja klienta HTTP z interceptorem dodającym token JWT do każdego żądania.

## Routing

Aplikacja używa **Vue Router** do dynamicznej zmiany widoków bez przeładowania strony.

- `/` – Strona główna z kalendarzem wydarzeń  
- `/login` – Formularz logowania  
- `/add` – Formularz dodawania wydarzenia  
- `/my-events` – Historia zgłoszeń zalogowanego użytkownika  
- `/pending` – Panel administratora (tylko dla adminów)  

## Backend

Backend zarządza użytkownikami i wydarzeniami za pomocą dwóch głównych kontrolerów:

- **AuthController** – logowanie (`/api/auth/login`) i rejestracja (`/api/auth/register`).  
- **EventController** – obsługa wydarzeń:  
  - Pobieranie listy wydarzeń i szczegółów (`GET`)  
  - Dodawanie wydarzeń (`POST`)  
  - Edycja i usuwanie (`PUT`, `DELETE`) – tylko dla administratorów  
  - Zatwierdzanie wydarzeń (`PUT /api/event/approve/{id}`) – tylko admin  

## Technologie

- Frontend: **Vue.js**, **Vue Router**, **Axios**  
- Backend: REST API  
- Autoryzacja: **JWT**

## Cel projektu
Projekt stworzony jako praca inżynierska.
