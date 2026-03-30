import { createRouter, createWebHistory } from 'vue-router'
import EventList from '../views/EventList.vue'
import Login from '../views/LoginView.vue'
import Register from '../views/RegisterView.vue'
import AddEvent from '../views/AddEvent.vue'
import PendingEvents from '../views/PendingEvents.vue'
import GalleryView from '../views/GalleryView.vue';
import MyEvents from '../views/MyEvents.vue';
import { jwtDecode } from "jwt-decode";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', component: EventList },
    { path: '/login', component: Login },
    { path: '/register', component: Register },
    { path: '/add', component: AddEvent, meta: { requiresAuth: true } },
    { path: '/my-events', component: MyEvents, meta: { requiresAuth: true } },
    { path: '/gallery', component: GalleryView },
    
    
    // TRASA DLA ADMINA
    { 
      path: '/pending', 
      component: PendingEvents, 
      meta: { requiresAuth: true, requiresAdmin: true } 
    }
  ]
})

// GUARD
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token');
  
  if (to.meta.requiresAuth && !token) {
    next('/login');
    return;
  }

  // Sprawdzanie Admina
  if (to.meta.requiresAdmin) {
    try {
      const decoded = jwtDecode(token);
      const role = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || decoded.role;
      
      if (role !== 'Admin') {
        alert("Brak uprawnień!");
        next('/'); // Przekieruj zwykłego usera na stronę główną
        return;
      }
    } catch (e) {
      next('/login');
      return;
    }
  }

  next();
});

export default router