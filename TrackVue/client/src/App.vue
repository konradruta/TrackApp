<template>
  <div id="app">
    <nav>
      <router-link to="/">Kalendarz</router-link>
      | <router-link to="/gallery">Galeria</router-link> <span v-if="isLoggedIn">
        
      </span>
      <span v-if="isLoggedIn">
        | <router-link to="/add">Dodaj wydarzenie</router-link>
        | <router-link to="my-events">Moje wydarzenia</router-link>
        
        <span v-if="isAdmin">
           | <router-link to="/pending" class="admin-link">⚠️ Do akceptacji</router-link>
        </span>

        | <a href="#" @click.prevent="logout" class="logout-btn">Wyloguj</a>
      </span>

      <span v-else>
        | <router-link to="/login">Logowanie</router-link> 
        | <router-link to="/register">Rejestracja</router-link>
      </span>
    </nav>
    
    <div v-if="userName" class="user-info">
      Zalogowano jako: <strong>{{ userName }}</strong>
    </div>

    <br/>
    <router-view />
  </div>
</template>

<script>
import { jwtDecode } from "jwt-decode"; // Importujemy bibliotekę

export default {
  data() {
    return {
      isLoggedIn: false,
      isAdmin: false,
      userName: ''
    };
  },
  mounted() {
    this.checkLoginStatus();
  },
  watch: {
    $route() {
      this.checkLoginStatus();
    }
  },
  methods: {
    checkLoginStatus() {
      const token = localStorage.getItem('token');
      
      if (token) {
        this.isLoggedIn = true;
        try {
          const decoded = jwtDecode(token);
          
          const role = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || decoded.role;
          
          this.isAdmin = (role === 'Admin');

          const name = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || decoded.unique_name;
          this.userName = name;

        } catch (e) {
          // Jeśli token jest uszkodzony
          console.error("Błąd dekodowania tokenu", e);
          this.logout();
        }
      } else {
        this.isLoggedIn = false;
        this.isAdmin = false;
        this.userName = '';
      }
    },
    logout() {
      localStorage.removeItem('token');
      this.isLoggedIn = false;
      this.isAdmin = false;
      this.userName = '';
      this.$router.push('/login');
    }
  }
}
</script>

<style>
/* Style globalne */
body {
  margin: 0;
  padding: 0;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  background-color: #f4f7f6;
}

#app {
  text-align: center;
}

nav {
  padding: 20px;
  background-color: #2c3e50; /* Ciemny pasek */
  box-shadow: 0 2px 5px rgba(0,0,0,0.1);
}

nav a {
  font-weight: bold;
  color: #ecf0f1; /* Jasny tekst */
  text-decoration: none;
  margin: 0 15px;
  font-size: 1.1rem;
  transition: color 0.3s;
}

nav a:hover {
  color: #42b983; /* Zielony po najechaniu */
}

nav a.router-link-exact-active {
  color: #42b983;
  border-bottom: 2px solid #42b983;
}

/* Styl dla przycisku wyloguj, żeby wyglądał jak link */
.logout-btn {
  cursor: pointer;
  color: #e74c3c !important; /* Czerwonawy kolor dla wyloguj */
}
.logout-btn:hover {
  color: #ff7675 !important;
}
.admin-link {
  color: #f39c12 !important; /* Pomarańczowy */
  border-bottom: 2px solid transparent;
}
.admin-link:hover {
  border-bottom: 2px solid #f39c12;
}

.user-info {
  text-align: right;
  font-size: 0.8rem;
  color: #777;
  padding-right: 20px;
}
</style>