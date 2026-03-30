<template>
  <div class="admin-container">
    
    <transition name="fade">
      <div v-if="notification.message" :class="['notification-banner', notification.type]">
        {{ notification.message }}
      </div>
    </transition>

    <div class="header">
      <h2>🛠️ Panel Zarządzania Wydarzeniami</h2>
      <div class="tabs">
        <button :class="{ active: filter === 1 }" @click="filter = 1">Oczekujące ({{ pendingCount }})</button>
        <button :class="{ active: filter === 2 }" @click="filter = 2">Zatwierdzone</button>
      </div>
    </div>

    <div v-if="loading" class="loading">Ładowanie danych...</div>
    
    <div v-else-if="filteredEvents.length === 0" class="empty-state">
      Brak wydarzeń w tej kategorii.
    </div>

    <div v-else class="events-list">
      <div v-for="event in filteredEvents" :key="event.id" class="admin-card" :class="'status-' + event.eventStatus">
        
        <div class="card-content">
          <div class="top-row">
            <h3>{{ event.eventName }}</h3>
            <span class="badge" v-if="event.eventStatus === 1">Nowe</span>
          </div>
          
          <p><strong>Typ:</strong> {{ getEventTypeName(event.eventType) }}</p>
          <p><strong>Termin:</strong> {{ formatDate(event.startDate) }} - {{ formatDate(event.endDate) }}</p>
          
          <div class="contact-info">
            <p><strong>👤 Zgłaszający:</strong> {{ event.organizer ? event.organizer.email : 'Brak danych' }}</p>
            <p><strong>📞 Telefon:</strong> {{ event.phoneNumber || 'Nie podano' }}</p>
          </div>
          
          <p class="desc"><strong>Opis:</strong> {{ event.description }}</p>
        </div>

        <div class="card-actions">
          <button v-if="event.eventStatus === 1" @click="approve(event.id)" class="btn btn-approve">
            Zatwierdź ✅
          </button>
          
          <button @click="openEditModal(event)" class="btn btn-edit">Edytuj ✏️</button>
          <button @click="remove(event.id)" class="btn btn-delete">Usuń 🗑️</button>
        </div>
      </div>
    </div>

    <div v-if="showModal" class="modal-overlay">
      <div class="modal-content">
        <h3>Edytuj wydarzenie</h3>
        <form @submit.prevent="saveEdit">
          <label>Nazwa:</label>
          <input v-model="editForm.eventName" required />

          <label>Typ:</label>
          <select v-model.number="editForm.eventType">
            <option value="1">Track Day</option>
            <option value="2">Szkolenie</option>
            <option value="3">Zawody</option>
            <option value="4">Zlot</option>
            <option value="5">Inne</option>
          </select>

          <label>Opis:</label>
          <textarea v-model="editForm.description"></textarea>

          <div class="row">
            <div class="col">
                <label>Start:</label>
                <input type="datetime-local" v-model="editForm.startDate" required />
            </div>
            <div class="col">
                <label>Koniec:</label>
                <input type="datetime-local" v-model="editForm.endDate" required />
            </div>
          </div>

          <div class="modal-actions">
            <button type="button" @click="showModal = false" class="btn btn-cancel">Anuluj</button>
            <button type="submit" class="btn btn-save">Zapisz zmiany</button>
          </div>
        </form>
      </div>
    </div>

  </div>
</template>

<script>
import axios from '../axios';

export default {
  data() {
    return {
      events: [],
      loading: true,
      filter: 1,
      showModal: false,
      editForm: {},
      notification: {
        message: '',
        type: ''
      }
    };
  },
  computed: {
    filteredEvents() {
      return this.events.filter(e => e.eventStatus === this.filter);
    },
    pendingCount() {
      return this.events.filter(e => e.eventStatus === 1).length;
    }
  },
  async mounted() {
    await this.fetchEvents();
  },
  methods: {
    showNotification(msg, type = 'success') {
      this.notification = { message: msg, type: type };
      setTimeout(() => {
        this.notification.message = '';
      }, 3000);
    },

    async fetchEvents() {
      try {
        const res = await axios.get('/api/event');
        this.events = res.data;
      } catch (err) {
        console.error("Błąd pobierania:", err);
        this.showNotification("Błąd pobierania danych z serwera", "error");
      } finally {
        this.loading = false;
      }
    },

    async approve(id) {
      if(!confirm("Zatwierdzić to wydarzenie?")) return;
      try {
        await axios.put(`/api/event/approve/${id}`);
        const ev = this.events.find(e => e.id === id);
        if(ev) ev.eventStatus = 2;

        this.showNotification("Pomyślnie zatwierdzono wydarzenie! ✅");
      } catch (err) {
        this.showNotification("Błąd: " + (err.response?.status === 403 ? "Brak uprawnień" : err.message), "error");
      }
    },

    async remove(id) {
      if(!confirm("Usunąć trwale?")) return;
      try {
        await axios.delete(`/api/event/${id}`);
        this.events = this.events.filter(e => e.id !== id);
        
        this.showNotification("Wydarzenie zostało usunięte 🗑️");
      } catch (err) {
        this.showNotification("Błąd: " + err.message, "error");
      }
    },

    openEditModal(event) {
      this.editForm = { ...event }; 
      this.showModal = true;
    },

    async saveEdit() {
      try {
        await axios.put(`/api/event/${this.editForm.id}`, this.editForm);
        
        this.showNotification("Zmiany zostały zapisane! 💾");
        this.showModal = false;
        await this.fetchEvents();
      } catch (err) {
        this.showNotification("Błąd edycji: " + err.message, "error");
      }
    },

    formatDate(date) { return new Date(date).toLocaleString('pl-PL'); },
    getEventTypeName(type) {
        const t = ["", "Track Day", "Szkolenie", "Zawody", "Zlot", "Inne"];
        return t[type] || "Inne";
    }
  }
};
</script>

<style scoped>
/* style powiadomień */
.notification-banner {
  position: fixed;
  top: 80px;
  left: 50%;
  transform: translateX(-50%);
  padding: 15px 30px;
  border-radius: 8px;
  color: white;
  font-weight: bold;
  z-index: 1000;
  box-shadow: 0 4px 15px rgba(0,0,0,0.2);
  min-width: 300px;
  text-align: center;
}
.notification-banner.success { background-color: #27ae60; }
.notification-banner.error { background-color: #c0392b; }

/* Animacja pojawiania się */
.fade-enter-active, .fade-leave-active { transition: opacity 0.5s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }

.admin-container { max-width: 900px; margin: 0 auto; padding: 20px; font-family: 'Segoe UI', sans-serif; }
.header { border-bottom: 2px solid #eee; margin-bottom: 20px; padding-bottom: 10px; }
.tabs button { background: none; border: none; padding: 10px 20px; font-size: 1rem; cursor: pointer; color: #777; font-weight: bold; }
.tabs button.active { border-bottom: 3px solid #3498db; color: #3498db; }

.admin-card { background: white; border: 1px solid #ddd; padding: 20px; margin-bottom: 15px; border-radius: 8px; display: flex; justify-content: space-between; align-items: start; box-shadow: 0 2px 5px rgba(0,0,0,0.05); }
.status-1 { border-left: 5px solid #f39c12; }
.status-2 { border-left: 5px solid #2ecc71; }

.card-content h3 { margin: 0 0 10px 0; display: flex; align-items: center; gap: 10px; }
.badge { font-size: 0.7rem; padding: 2px 6px; border-radius: 4px; background: #f39c12; color: white; text-transform: uppercase; }
.badge.green { background: #2ecc71; }
.card-actions { display: flex; flex-direction: column; gap: 8px; }

.btn { padding: 8px 12px; border: none; border-radius: 4px; cursor: pointer; color: white; font-weight: bold; }
.btn-approve { background: #27ae60; }
.btn-edit { background: #3498db; }
.btn-delete { background: #c0392b; }

.modal-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); display: flex; justify-content: center; align-items: center; z-index: 999; }
.modal-content { background: white; padding: 30px; border-radius: 8px; width: 90%; max-width: 500px; }
.modal-content input, .modal-content select, .modal-content textarea { width: 100%; padding: 8px; margin-bottom: 10px; border: 1px solid #ccc; box-sizing: border-box; }
.row { display: flex; gap: 10px; } .col { flex: 1; }
.modal-actions { display: flex; justify-content: flex-end; gap: 10px; margin-top: 15px; }
.btn-cancel { background: #95a5a6; } .btn-save { background: #2ecc71; }
</style>