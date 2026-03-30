<template>
  <div class="page-bg">
    <div class="container">
      <h2>Moje wnioski o rezerwację</h2>

      <div v-if="loading" class="loading">
        Ładowanie danych...
      </div>

      <div v-else-if="myEvents.length === 0" class="empty-state">
        <p>Nie znaleziono wniosków dla Twojego konta.</p>
        <p style="font-size: 0.9rem; color: #999;">(Twoje ID: {{ myId }})</p>
        <router-link to="/add" class="add-btn">Złóż wniosek</router-link>
      </div>

      <div v-else class="table-responsive">
        <table class="events-table">
          <thead>
            <tr>
              <th>Nazwa</th>
              <th>Data rozpoczęcia</th>
              <th>Data zakończenia</th>
              <th>Typ</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="event in myEvents" :key="event.id">
              <td><strong>{{ event.eventName }}</strong></td>
              <td>{{ formatDate(event.startDate) }}</td>
              <td>{{ formatDate(event.endDate) }}</td>
              <td>{{ getEventTypeLabel(event.eventType) }}</td>
              <td>
                <span :class="getStatusClass(event.eventStatus)">
                  {{ getStatusLabel(event.eventStatus) }}
                </span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import axios from '../axios';

export default {
  data() {
    return {
      myEvents: [],
      loading: true,
      myId: ''
    };
  },
  async mounted() {
    await this.fetchMyEvents();
  },
  methods: {
    async fetchMyEvents() {
      const currentUserId = localStorage.getItem('userId');
      this.myId = currentUserId;
      
      if (!currentUserId) {
        this.$router.push('/login');
        return;
      }

      try {
        const res = await axios.get('/api/event');
        
        console.log("Dane z API:", res.data);

        this.myEvents = res.data.filter(event => 
          event.organizer && (event.organizer.id == currentUserId)
        );
        
        this.myEvents.sort((a, b) => new Date(b.startDate) - new Date(a.startDate));

      } catch (err) {
        console.error("Błąd pobierania historii:", err);
      } finally {
        this.loading = false;
      }
    },

    formatDate(dateString) {
      if (!dateString) return '-';
      const options = { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' };
      return new Date(dateString).toLocaleDateString('pl-PL', options);
    },

    getStatusLabel(status) {
      switch (status) {
        case 1: return 'Oczekuje';
        case 2: return 'Zatwierdzone';
        case 3: return 'Odrzucone';
        default: return 'Nieznany';
      }
    },
    
    getStatusClass(status) {
      switch (status) {
        case 1: return 'status-pending';
        case 2: return 'status-approved';
        case 3: return 'status-rejected';
        default: return '';
      }
    },

    getEventTypeLabel(type) {
      const types = {
        1: 'Track Day',
        2: 'Szkolenie',
        3: 'Zawody',
        4: 'Zlot',
        5: 'Inne'
      };
      return types[type] || 'Inne';
    }
  }
};
</script>

<style scoped>
.page-bg { background-color: #f4f7f6; min-height: 90vh; padding: 40px 20px; }
.container { max-width: 900px; margin: 0 auto; background: white; padding: 30px; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); }
h2 { margin-bottom: 25px; color: #333; text-align: center; }

.table-responsive { overflow-x: auto; }
.events-table { width: 100%; border-collapse: collapse; margin-top: 10px; }
.events-table th, .events-table td { padding: 12px 15px; text-align: left; border-bottom: 1px solid #ddd; }
.events-table th { background-color: #f8f9fa; font-weight: 600; color: #555; }
.events-table tr:hover { background-color: #f1f1f1; }

.status-pending { background-color: #fff3cd; color: #856404; padding: 5px 10px; border-radius: 20px; font-size: 0.85rem; font-weight: bold; }
.status-approved { background-color: #d4edda; color: #155724; padding: 5px 10px; border-radius: 20px; font-size: 0.85rem; font-weight: bold; }
.status-rejected { background-color: #f8d7da; color: #721c24; padding: 5px 10px; border-radius: 20px; font-size: 0.85rem; font-weight: bold; }

.loading { text-align: center; color: #777; font-size: 1.2rem; padding: 40px; }
.empty-state { text-align: center; padding: 40px; color: #666; }
.add-btn { display: inline-block; margin-top: 15px; padding: 10px 20px; background-color: #42b983; color: white; text-decoration: none; border-radius: 6px; font-weight: bold; }
.add-btn:hover { background-color: #369b6d; }
</style>