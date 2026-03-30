<template>
  <div class="calendar-wrapper">
    
    <div class="filters-bar">
      <div class="filter-group">
        <label>🔍 Szukaj wydarzenia:</label>
        <input 
          type="text" 
          v-model="searchQuery" 
          placeholder="np. Drift, Trening..." 
        />
      </div>

      <div class="filter-group">
        <label>🏎️ Typ imprezy:</label>
        <select v-model="selectedType">
          <option value="">Wszystkie typy</option>
          <option :value="1">Track Day</option>
          <option :value="2">Szkolenie</option>
          <option :value="3">Zawody</option>
          <option :value="4">Zlot</option>
          <option :value="5">Inne</option>
        </select>
      </div>
      
      <button v-if="searchQuery || selectedType" @click="resetFilters" class="reset-btn">
        Wyczyść filtry ✕
      </button>
    </div>

    <div class="calendar-header">
      <button class="nav-btn" @click="changeMonth(-1)">&lt;</button>
      <h2>{{ currentMonthName }} {{ currentYear }}</h2>
      <button class="nav-btn" @click="changeMonth(1)">&gt;</button>
    </div>

    <div class="weekdays-grid">
      <div v-for="day in weekDays" :key="day" class="weekday">{{ day }}</div>
    </div>

    <div class="days-grid">
      <div v-for="n in blankDays" :key="'blank-'+n" class="day-cell empty"></div>

      <div v-for="day in daysInMonth" :key="day" class="day-cell">
        <span class="day-number" :class="{ 'today': isToday(day) }">{{ day }}</span>
        
        <div class="events-stack">
          <div 
            v-for="event in getEventsForDay(day)" 
            :key="event.id" 
            class="event-pill"
            :class="getEventClass(event.eventType)"
            @click="showDetails(event)"
            :title="event.eventName"
          >
            {{ event.eventName }}
          </div>
        </div>
      </div>
    </div>
  </div>
  <div v-if="selectedEvent" class="modal-backdrop" @click.self="closeModal">
      <div class="modal-content">
        <button class="close-btn" @click="closeModal">&times;</button>
        
        <h3>{{ selectedEvent.eventName }}</h3>
        
        <div class="modal-body">
          <p><strong>Typ:</strong> {{ getEventTypeLabel(selectedEvent.eventType) }}</p>
          <p><strong>Start:</strong> {{ formatDate(selectedEvent.startDate) }}</p>
          <p><strong>Koniec:</strong> {{ formatDate(selectedEvent.endDate) }}</p>
          
          <hr />
          
          <p class="description-label">Opis:</p>
          <p class="description-text">
            {{ selectedEvent.description || 'Brak dodatkowego opisu.' }}
          </p>
        </div>

        <div class="modal-footer">
          <button class="ok-btn" @click="closeModal">Zamknij</button>
        </div>
      </div>
    </div>
</template>

<script>
import axios from '../axios';
import { jwtDecode } from "jwt-decode";

export default {
  data() {
    return {
      currentDate: new Date(),
      events: [],
      weekDays: ['Pon', 'Wt', 'Śr', 'Czw', 'Pt', 'Sob', 'Niedz'],
      
      searchQuery: '',
      selectedType: '',
      selectedEvent: null 
    };
  },
  computed: {
    currentYear() { return this.currentDate.getFullYear(); },
    currentMonth() { return this.currentDate.getMonth(); },
    currentMonthName() { return this.currentDate.toLocaleString('pl-PL', { month: 'long' }); },
    daysInMonth() { return new Date(this.currentYear, this.currentMonth + 1, 0).getDate(); },
    blankDays() {
      const firstDayIndex = new Date(this.currentYear, this.currentMonth, 1).getDay();
      return firstDayIndex === 0 ? 6 : firstDayIndex - 1;
    },

    filteredEvents() {
      return this.events.filter(event => {
        const matchName = event.eventName.toLowerCase().includes(this.searchQuery.toLowerCase());
        
        const matchType = this.selectedType === '' || event.eventType === this.selectedType;

        return matchName && matchType;
      });
    }
  },
  async mounted() {
    await this.fetchEvents();
  },
  methods: {
    async fetchEvents() {
      try {
        const res = await axios.get('/api/event');
        const allEvents = res.data;

        const isAdmin = this.checkIfAdmin(); 

        if (isAdmin) {
          this.events = allEvents;
        } else {
          this.events = allEvents.filter(e => e.eventStatus === 2);
        }

      } catch (err) {
        console.error("Błąd pobierania wydarzeń", err);
      }
    },
    
    getEventsForDay(day) {
      const cellDate = new Date(this.currentYear, this.currentMonth, day, 12, 0, 0);

      return this.filteredEvents.filter(event => {
        const start = new Date(event.startDate);
        start.setHours(0, 0, 0, 0);
        const end = new Date(event.endDate);
        end.setHours(23, 59, 59, 999);

        return cellDate >= start && cellDate <= end;
      });
    },

    resetFilters() {
      this.searchQuery = '';
      this.selectedType = '';
    },

    changeMonth(step) {
      this.currentDate = new Date(this.currentYear, this.currentMonth + step, 1);
    },
    isToday(day) {
      const today = new Date();
      return day === today.getDate() &&
             this.currentMonth === today.getMonth() &&
             this.currentYear === today.getFullYear();
    },
    getEventClass(type) {
      switch (type) {
        case 1: return 'evt-trackday';
        case 2: return 'evt-training';
        case 3: return 'evt-race';
        case 4: return 'evt-meetup';
        case 5: return 'evt-other';
        default: return 'evt-default';
      }
    },
    showDetails(event) {
      this.selectedEvent = event;
    },
    closeModal() {
      this.selectedEvent = null;
    },
    getEventTypeLabel(typeId) {
      const types = ["", "Track Day", "Szkolenie", "Zawody", "Zlot", "Inne"];
      return types[typeId] || "Nieznany";
    },

    formatDate(dateString) {
      if (!dateString) return '-';
      return new Date(dateString).toLocaleString('pl-PL', {
        year: 'numeric', month: 'long', day: 'numeric',
        hour: '2-digit', minute: '2-digit'
      })},
    checkIfAdmin() {
        const token = localStorage.getItem('token');
        if (!token) return false;
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            const role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || payload.role;
            return role === 'Admin';
        } catch {
            return false;
        }
    }
  }
};
</script>

<style scoped>
.calendar-wrapper { max-width: 1200px; margin: 20px auto; padding: 20px; font-family: 'Segoe UI', sans-serif; }

.filters-bar {
  display: flex;
  gap: 20px;
  background-color: white;
  padding: 15px;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
  margin-bottom: 20px;
  flex-wrap: wrap;
  align-items: flex-end;
}

.filter-group {
  display: flex;
  flex-direction: column;
}

.filter-group label {
  font-size: 0.85rem;
  font-weight: bold;
  color: #666;
  margin-bottom: 5px;
}

.filters-bar input, .filters-bar select {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 1rem;
  min-width: 200px;
}

.reset-btn {
  background: none;
  border: 1px solid #e74c3c;
  color: #e74c3c;
  padding: 8px 15px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: bold;
  transition: 0.2s;
  height: 38px;
}

.modal-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  padding: 25px;
  border-radius: 12px;
  width: 90%;
  max-width: 500px;
  box-shadow: 0 5px 30px rgba(0,0,0,0.3);
  position: relative;
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(-20px); }
  to { opacity: 1; transform: translateY(0); }
}

.modal-content h3 {
  margin-top: 0;
  color: #333;
  border-bottom: 2px solid #42b983;
  padding-bottom: 10px;
  margin-bottom: 20px;
}

.modal-body p {
  margin: 10px 0;
  color: #555;
  line-height: 1.5;
}

.description-label {
  font-weight: bold;
  margin-bottom: 5px !important;
}

.description-text {
  background-color: #f9f9f9;
  padding: 10px;
  border-radius: 6px;
  font-style: italic;
}

.close-btn {
  position: absolute;
  top: 15px;
  right: 15px;
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #999;
}
.close-btn:hover { color: #333; }

.modal-footer {
  margin-top: 20px;
  text-align: right;
}

.ok-btn {
  background-color: #42b983;
  color: white;
  border: none;
  padding: 10px 25px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: bold;
  transition: 0.2s;
}
.ok-btn:hover {
  background-color: #369b6d;
}
.reset-btn:hover { background-color: #e74c3c; color: white; }

.calendar-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
.calendar-header h2 { text-transform: capitalize; margin: 0; color: #2c3e50; font-size: 1.8rem; }
.nav-btn { background: white; border: 1px solid #ddd; padding: 8px 16px; font-size: 1.2rem; cursor: pointer; border-radius: 6px; transition: 0.2s; }
.nav-btn:hover { background-color: #f0f0f0; }

.weekdays-grid, .days-grid { display: grid; grid-template-columns: repeat(7, 1fr); gap: 1px; background-color: #ddd; border: 1px solid #ddd; }
.weekday { background-color: #f8f9fa; text-align: center; padding: 10px; font-weight: bold; color: #666; text-transform: uppercase; font-size: 0.85rem; }

.day-cell { background-color: white; min-height: 120px; padding: 5px; position: relative; transition: 0.2s; }
.day-cell:hover { background-color: #fafafa; }
.day-cell.empty { background-color: #f9f9f9; }

.day-number { font-size: 0.9rem; font-weight: 600; color: #777; padding: 4px 8px; border-radius: 50%; display: inline-block; }
.day-number.today { background-color: #42b983; color: white; }

.events-stack { display: flex; flex-direction: column; gap: 3px; margin-top: 5px; }
.event-pill { font-size: 0.75rem; padding: 3px 6px; border-radius: 4px; color: white; cursor: pointer; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; opacity: 0.9; }
.event-pill:hover { opacity: 1; transform: scale(1.01); }



.evt-trackday { background-color: #27ae60; }
.evt-training { background-color: #2980b9; }
.evt-race     { background-color: #d35400; }
.evt-meetup   { background-color: #8e44ad; }
.evt-other    { background-color: #7f8c8d; }
.evt-default  { background-color: #2ecc71; }
</style>