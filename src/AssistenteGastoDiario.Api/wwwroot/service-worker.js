const CACHE_NAME = "assistente-gasto-diario-v3";
const APP_SHELL = [
    "/",
    "/index.html",
    "/styles.css",
    "/app.js",
    "/manifest.webmanifest",
    "/icons/icon.svg"
];

self.addEventListener("install", (event) => {
    event.waitUntil(
        caches.open(CACHE_NAME).then((cache) => cache.addAll(APP_SHELL))
    );
    self.skipWaiting();
});

self.addEventListener("activate", (event) => {
    event.waitUntil(
        caches.keys().then((cacheNames) => Promise.all(
            cacheNames
                .filter((cacheName) => cacheName !== CACHE_NAME)
                .map((cacheName) => caches.delete(cacheName))
        ))
    );
    self.clients.claim();
});

self.addEventListener("fetch", (event) => {
    const request = event.request;
    const url = new URL(request.url);

    if (url.origin !== self.location.origin || request.method !== "GET") {
        return;
    }

    if (url.pathname.startsWith("/api/") || url.pathname === "/health") {
        return;
    }

    if (request.mode === "navigate") {
        event.respondWith(
            fetch(request).catch(() => caches.match("/index.html"))
        );
        return;
    }

    event.respondWith(networkFirst(request));
});

async function networkFirst(request) {
    const cache = await caches.open(CACHE_NAME);

    try {
        const response = await fetch(request);
        if (response.ok) {
            cache.put(request, response.clone());
        }

        return response;
    } catch {
        return cache.match(request);
    }
}
