<template>
  <div class="register-container">
    <div class="register-card">
      <div class="register-header">
        <h2>Załóż konto</h2>
        <p>Dołącz do społeczności kierowców</p>
      </div>
      
      <form @submit.prevent="handleRegister">
        
        <div class="input-group">
          <label>Nazwa użytkownika</label>
          <input 
            type="text" 
            v-model="form.userName" 
            placeholder="np. SzybkiKierowca99" 
            required 
            minlength="3"
          />
        </div>

        <div class="input-group">
          <label>Adres Email</label>
          <input 
            type="email" 
            v-model="form.email" 
            placeholder="jan@kowalski.pl" 
            required 
          />
        </div>
        
        <div class="input-group">
          <label>Hasło</label>
          <input 
            type="password" 
            v-model="form.password" 
            placeholder="Min. 6 znaków" 
            required 
          />
        </div>

        <div class="input-group">
          <label>Potwierdź hasło</label>
          <input 
            type="password" 
            v-model="form.confirmPassword" 
            placeholder="Powtórz hasło" 
            required 
          />
        </div>
        
        <button type="submit" class="register-btn">Zarejestruj się</button>
      </form>
      
      <p v-if="error" class="error-msg">{{ error }}</p>
      
      <div class="login-link">
        Masz już konto? <router-link to="/login">Zaloguj się</router-link>
      </div>
    </div>
  </div>
</template>

<script>
import axios from '../axios';

export default {
  data() {
    return {
      form: {
        userName: '', 
        email: '',
        password: '',
        confirmPassword: ''
      },
      error: ''
    };
  },
  methods: {
    async handleRegister() {
      this.error = '';
      
      if (this.form.password !== this.form.confirmPassword) {
        this.error = "Hasła nie są identyczne!";
        return;
      }

      try {
        await axios.post('/api/auth/register', this.form);
        
        alert("Konto zostało utworzone! Możesz się teraz zalogować.");
        this.$router.push('/login');
        
      } catch (err) {
        if (err.response && err.response.data) {
           console.error("Błąd rejestracji:", err.response.data);
           this.error = "Nie udało się zarejestrować. Sprawdź dane (np. czy email/nick jest unikalny).";
        } else {
           this.error = "Wystąpił błąd połączenia z serwerem.";
        }
      }
    }
  }
};
</script>

<style scoped>
.register-container { display: flex; justify-content: center; align-items: center; min-height: 80vh; background-color: #f4f7f6; }
.register-card { background: white; padding: 40px; border-radius: 12px; box-shadow: 0 4px 20px rgba(0,0,0,0.1); width: 100%; max-width: 400px; text-align: center; }
.register-header h2 { margin-bottom: 5px; color: #333; }
.register-header p { color: #777; margin-bottom: 30px; font-size: 0.9rem; }
.input-group { text-align: left; margin-bottom: 15px; }
.input-group label { display: block; margin-bottom: 5px; font-weight: 600; color: #444; font-size: 0.9rem; }
.input-group input { width: 100%; padding: 10px; border: 1px solid #ddd; border-radius: 6px; box-sizing: border-box; }
.register-btn { width: 100%; padding: 12px; background-color: #3498db; color: white; border: none; border-radius: 6px; font-weight: bold; cursor: pointer; transition: 0.3s; margin-top: 10px; }
.register-btn:hover { background-color: #2980b9; }
.error-msg { color: #e74c3c; margin-top: 15px; font-size: 0.9rem; }
.login-link { margin-top: 20px; font-size: 0.9rem; }
.login-link a { color: #3498db; text-decoration: none; font-weight: bold; }
</style>