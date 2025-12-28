// js/scripts.js
document.addEventListener('DOMContentLoaded', () => {
    // yıl
    document.getElementById('year')?.textContent = new Date().getFullYear();

    // Theme toggle
    const themeBtn = document.getElementById('themeToggle');
    themeBtn?.addEventListener('click', () => {
        document.body.classList.toggle('dark-mode');
        document.body.classList.toggle('light-mode');
        themeBtn.innerHTML = document.body.classList.contains('dark-mode') ? '<i class="fa fa-sun"></i>' : '<i class="fa fa-moon"></i>';
    });

    // Search button -> sayfa jobs.html'da filtre uygulasın
    document.getElementById('searchBtn')?.addEventListener('click', () => {
        const q = document.getElementById('searchInput').value.trim();
        if (!q) { showToast('Arama terimi girin', 'warning'); return; }
        // MVC ile: window.location = `/Jobs?query=${encodeURIComponent(q)}`;
        showToast(`Arama: ${q}`, 'info');
    });

    // Toast helper
    window.showToast = (message, type = 'success') => {
        const container = document.getElementById('toastContainer');
        const el = document.createElement('div');
        el.className = `toast align-items-center text-bg-${type} border-0 mb-2`;
        el.role = 'alert';
        el.ariaLive = 'assertive';
        el.ariaAtomic = 'true';
        el.innerHTML = `<div class="d-flex">
      <div class="toast-body">${message}</div>
      <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
    </div>`;
        container.appendChild(el);
        const bs = new bootstrap.Toast(el);
        bs.show();
        el.addEventListener('hidden.bs.toast', () => el.remove());
    };

    // CV preview (file input with data-preview attr)
    document.querySelectorAll('input[type=file][data-preview]').forEach(inp => {
        inp.addEventListener('change', e => {
            const file = e.target.files[0];
            const target = document.querySelector(inp.dataset.preview);
            if (!file) return;
            if (file.type.startsWith('image/')) {
                const url = URL.createObjectURL(file);
                target.src = url;
            } else {
                target.textContent = file.name;
            }
        });
    });

    // Example Chart in dashboard
    const ctx = document.getElementById('applicationsChart');
    if (ctx) {
        new Chart(ctx, {
            type: 'line',
            data: {
                labels: ['May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct'],
                datasets: [{ label: 'Başvurular', data: [12, 19, 7, 25, 20, 30], fill: true }]
            },
            options: { responsive: true, plugins: { legend: { display: false } } }
        });
    }
});
