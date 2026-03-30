<template>
  <div class="login-container">
    <div class="login-card">
      <div class="login-header">
        <h2>Witaj ponownie!</h2>
        <p>Zaloguj się do systemu zarządzania torem</p>
      </div>
      
      <form @submit.prevent="handleLogin">
        <div class="input-group">
          <label>Adres Email</label>
          <input 
            type="email" 
            v-model="email" 
            placeholder="np. admin@track.com" 
            required 
          />
        </div>
        
        <div class="input-group">
          <label>Hasło</label>
          <input 
            type="password" 
            v-model="password" 
            placeholder="••••••••" 
            required 
          />
        </div>
        
        <button type="submit" class="login-btn">Zaloguj się</button>
      </form>
      
      <p v-if="error" class="error-msg">{{ error }}</p>
    </div>
  </div>
</template>

<script>
import axios from '../axios';

export default {
  data() { return { email: '', password: '', error: '' }; },
  methods: {
    parseJwt(token) {
      try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(jsonPayload);
      } catch (e) {
        return null;
      }
    },

    async handleLogin() {
      this.error = '';
      try {
        const response = await axios.post('/api/auth/login', {
          email: this.email,
          password: this.password
        });

        const token = response.data;
        
        const decodedToken = this.parseJwt(token);
        console.log("Rozkodowany Token:", decodedToken);

        let userId = decodedToken.nameid 
                  || decodedToken.sub 
                  || decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']
                  || decodedToken.id;

        if (!userId) {
             console.error("Nie znaleziono ID w tokenie! Dostępne pola:", Object.keys(decodedToken));
             this.error = "Błąd autoryzacji: Brak ID użytkownika w tokenie.";
             return;
        }

        localStorage.setItem('token', token);
        localStorage.setItem('userId', userId);
        
        console.log("Zalogowano pomyślnie. UserId:", userId);
        this.$router.push('/');

      } catch (err) {
        console.error(err);
        this.error = "Błędny login, hasło lub błąd serwera.";
      }
    }
  }
};
</script>

<style scoped>
/* Tło całego ekranu */
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 80vh; 
  background-color: #f4f7f6;
}

/* Karta logowania */
.login-card {
  background: white;
  padding: 40px;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0,0,0,0.1);
  width: 100%;
  max-width: 400px;
  text-align: center;
}

.login-header h2 { margin-bottom: 5px; color: #333; }
.login-header p { color: #777; margin-bottom: 30px; font-size: 0.9rem; }

/* Pola formularza */
.input-group { text-align: left; margin-bottom: 20px; }
.input-group label { display: block; margin-bottom: 8px; font-weight: 600; color: #444; font-size: 0.9rem; }
.input-group input {
  width: 100%;
  padding: 12px;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 1rem;
  transition: border-color 0.3s;
  box-sizing: border-box; 
}
.input-group input:focus { border-color: #42b983; outline: none; }

/* Przycisk */
.login-btn {
  width: 100%;
  padding: 12px;
  background-color: #42b983;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.3s;
}
.login-btn:hover { background-color: #369b6d; }

.error-msg { color: #e74c3c; margin-top: 15px; font-size: 0.9rem; }
</style>