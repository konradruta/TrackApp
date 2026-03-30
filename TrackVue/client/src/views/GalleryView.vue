<template>
  <div class="gallery-container">
    <div class="gallery-header">
      <h2>🏎️ Galeria Toru</h2>
      <p>Zobacz emocje, które towarzyszą naszym wydarzeniom!</p>
    </div>

    <div class="gallery-grid">
      <div 
        v-for="(img, index) in images" 
        :key="index" 
        class="gallery-item"
        @click="openLightbox(index)"
      >
        <img :src="img.src" :alt="img.alt" loading="lazy" />
        <div class="overlay">
          <span>{{ img.alt }}</span>
        </div>
      </div>
    </div>

    <div v-if="lightboxOpen" class="lightbox" @click="closeLightbox">
      <span class="close-btn">&times;</span>
      <img :src="images[currentImageIndex].src" class="lightbox-img" @click.stop />
      <button class="prev" @click.stop="prevImage">&#10094;</button>
      <button class="next" @click.stop="nextImage">&#10095;</button>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      lightboxOpen: false,
      currentImageIndex: 0,
      images: [
        { 
          src: 'https://images.unsplash.com/photo-1568605117036-5fe5e7bab0b7?auto=format&fit=crop&w=800&q=80', 
          alt: 'Drift Show' 
        },
        { 
          src: 'https://img.redbull.com/images/q_auto,f_auto/redbullcom/2013/04/01/1331585412344_1/fia-gt-series-runda-1?auto=format&fit=crop&w=800&q=80', 
          alt: 'Wyścigi GT' 
        },
        { 
          src: 'https://images.unsplash.com/photo-1552519507-da3b142c6e3d?auto=format&fit=crop&w=800&q=80', 
          alt: 'Zlot Klasyków' 
        },
        { 
          src: 'https://images.unsplash.com/photo-1494976388531-d1058494cdd8?auto=format&fit=crop&w=800&q=80', 
          alt: 'Track Day - Poranek' 
        },
        { 
          src: 'https://images.unsplash.com/photo-1492144534655-ae79c964c9d7?auto=format&fit=crop&w=800&q=80', 
          alt: 'Szkolenie bezpiecznej jazdy' 
        },
        { 
          src: 'https://images.unsplash.com/photo-1542282088-fe8426682b8f?auto=format&fit=crop&w=800&q=80', 
          alt: 'Nocna Jazda' 
        },
      ]
    };
  },
  methods: {
    openLightbox(index) {
      this.currentImageIndex = index;
      this.lightboxOpen = true;
    },
    closeLightbox() {
      this.lightboxOpen = false;
    },
    nextImage() {
      this.currentImageIndex = (this.currentImageIndex + 1) % this.images.length;
    },
    prevImage() {
      this.currentImageIndex = (this.currentImageIndex - 1 + this.images.length) % this.images.length;
    }
  }
};
</script>

<style scoped>
.gallery-container { max-width: 1200px; margin: 0 auto; padding: 20px; font-family: 'Segoe UI', sans-serif; }
.gallery-header { text-align: center; margin-bottom: 40px; }
.gallery-header h2 { color: #2c3e50; font-size: 2rem; margin-bottom: 10px; }
.gallery-header p { color: #7f8c8d; }

/* Grid zdjęć */
.gallery-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 15px;
}

.gallery-item {
  position: relative;
  overflow: hidden;
  border-radius: 8px;
  cursor: pointer;
  box-shadow: 0 4px 6px rgba(0,0,0,0.1);
  aspect-ratio: 16/10; /* Utrzymuje proporcje */
}

.gallery-item img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.4s ease;
}

.gallery-item:hover img {
  transform: scale(1.1);
}

.overlay {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  background: rgba(0,0,0,0.6);
  color: white;
  padding: 10px;
  transform: translateY(100%);
  transition: transform 0.3s ease;
  text-align: center;
}

.gallery-item:hover .overlay {
  transform: translateY(0);
}

/* Lightbox */
.lightbox {
  position: fixed; top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(0,0,0,0.9);
  display: flex; justify-content: center; align-items: center;
  z-index: 1000;
}
.lightbox-img { max-width: 90%; max-height: 90%; border-radius: 4px; box-shadow: 0 0 20px rgba(0,0,0,0.5); }
.close-btn { position: absolute; top: 20px; right: 30px; color: white; font-size: 40px; cursor: pointer; }
.prev, .next { position: absolute; top: 50%; transform: translateY(-50%); background: none; border: none; color: white; font-size: 3rem; cursor: pointer; padding: 20px; }
.prev { left: 20px; }
.next { right: 20px; }
.prev:hover, .next:hover { color: #3498db; }
</style>