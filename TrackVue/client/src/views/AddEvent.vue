<template>
  <div class="form-bg">
    <div class="form-container">
      <h2>Dodaj nowe wydarzenie</h2>
      
      <div v-if="errorMessage" class="message-banner error">
        ⚠️ {{ errorMessage }}
      </div>

      <div v-if="successMessage" class="message-banner success">
        ✅ {{ successMessage }}
      </div>

      <form @submit.prevent="validateAndAdd">
        
        <div class="form-group">
          <label>Nazwa wydarzenia</label>
          <input v-model="form.eventName" placeholder="Np. Track Day Tor Poznań" required />
        </div>

        <div class="form-group">
          <label>Typ wydarzenia</label>
          <select v-model.number="form.eventType" required>
            <option disabled value="">Wybierz z listy...</option>
            <option value="1">Track Day</option>
            <option value="2">Szkolenie</option>
            <option value="3">Zawody</option>
            <option value="4">Zlot</option>
            <option value="5">Inne</option>
          </select>
        </div>

        <div class="form-group">
          <label>Opis</label>
          <textarea v-model="form.description" placeholder="Szczegóły..."></textarea>
        </div>

        <div class="row">
          <div class="form-group half">
            <label>Start</label>
            <input type="datetime-local" v-model="form.startDate" required />
          </div>
          <div class="form-group half">
            <label>Koniec</label>
            <input type="datetime-local" v-model="form.endDate" required />
          </div>
        </div>

        <div class="form-group">
          <label>Telefon kontaktowy</label>
            <input 
              type="tel" 
              v-model="form.phoneNumber" 
              placeholder="np. 500 123 456" 
              required 
                  />
          </div>

        <button type="submit" class="submit-btn" :disabled="isSubmitting">
          {{ isSubmitting ? 'Wysyłanie...' : 'Zapisz wydarzenie' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script>
import axios from '../axios';

export default {
  data() {
    return {
      form: {
        eventName: '',
        description: '',
        startDate: '',
        endDate: '',
        eventType: 1,
        eventStatus: 1,
        phoneNumber: '',
        organizerId: '' 
      },
      existingEvents: [], 
      errorMessage: '',
      successMessage: '',
      isSubmitting: false
    };
  },
  created() {
    const storedUserId = localStorage.getItem('userId');
    if (storedUserId) {
      this.form.organizerId = storedUserId;
    } else {
      console.warn('Nie znaleziono userId w localStorage.');
    }
  },
  async mounted() {
    await this.fetchExistingEvents();
  },
  methods: {
    async fetchExistingEvents() {
      try {
        const res = await axios.get('/api/event');
        this.existingEvents = res.data.filter(e => e.eventStatus === 2);
      } catch (err) {
        console.error("Nie udało się pobrać wydarzeń", err);
      }
    },

    validateAndAdd() {
      this.errorMessage = '';
      this.successMessage = '';

      if (!this.form.organizerId) {
        this.errorMessage = "Błąd: Nie zidentyfikowano użytkownika. Zaloguj się ponownie.";
        return;
      }

      const start = new Date(this.form.startDate);
      const end = new Date(this.form.endDate);
      const now = new Date();

      if (start >= end) {
        this.errorMessage = "Data zakończenia musi być późniejsza niż data rozpoczęcia.";
        return;
      }

      if (start < now) {
        this.errorMessage = "Nie można dodać wydarzenia w przeszłości.";
        return;
      }

      const hasConflict = this.existingEvents.some(event => {
        const existingStart = new Date(event.startDate);
        const existingEnd = new Date(event.endDate);
        return (start < existingEnd && end > existingStart);
      });

      if (hasConflict) {
        this.errorMessage = "W tym terminie istnieje już inne zatwierdzone wydarzenie! Wybierz inną datę.";
        return;
      }

      this.add();
    },

    async add() {
      this.isSubmitting = true;
      try {
        await axios.post('/api/event/add', this.form);
        
        // SUKCES:
        this.successMessage = 'Wydarzenie zostało pomyślnie wysłane do akceptacji!';
        
        const savedId = this.form.organizerId;
        this.form = {
          eventName: '',
          description: '',
          startDate: '',
          endDate: '',
          eventType: 1,
          eventStatus: 1,
          phoneNumber: '',
          organizerId: savedId 
        };

        window.scrollTo({ top: 0, behavior: 'smooth' });

      } catch (err) {
        console.error(err);
        this.errorMessage = 'Błąd serwera. Spróbuj ponownie.';
      } finally {
        this.isSubmitting = false;
      }
    }
  }
};
</script>

<style scoped>
.form-bg { background-color: #f4f7f6; min-height: 90vh; display: flex; justify-content: center; align-items: center; padding: 20px; }
.form-container { background: white; padding: 30px; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); width: 100%; max-width: 500px; }
h2 { text-align: center; margin-bottom: 25px; color: #333; }
.form-group { margin-bottom: 15px; }
.form-group label { display: block; margin-bottom: 6px; font-weight: 600; color: #555; font-size: 0.9rem; }
.form-group input, .form-group select, .form-group textarea { width: 100%; padding: 10px; border: 1px solid #ddd; border-radius: 6px; font-size: 1rem; box-sizing: border-box; }
.form-group textarea { resize: vertical; min-height: 80px; }
.row { display: flex; gap: 15px; }
.half { flex: 1; }
.submit-btn { width: 100%; padding: 12px; background-color: #42b983; color: white; border: none; border-radius: 6px; font-size: 1rem; font-weight: bold; cursor: pointer; transition: 0.3s; margin-top: 10px; }
.submit-btn:hover { background-color: #369b6d; }
.submit-btn:disabled { background-color: #ccc; cursor: not-allowed; }

/* Uniwersalne style dla banerów */
.message-banner {
  padding: 12px;
  border-radius: 6px;
  margin-bottom: 20px;
  font-weight: bold;
  text-align: center;
}

.message-banner.error {
  background-color: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
}

.message-banner.success {
  background-color: #d4edda;
  color: #155724;
  border: 1px solid #c3e6cb;
}
</style>