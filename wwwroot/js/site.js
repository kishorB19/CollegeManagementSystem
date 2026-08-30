/* ============================================
   CMS Portal — Main JavaScript
   Technologies: HTML5, CSS3, Vanilla JavaScript
   ============================================ */

document.addEventListener('DOMContentLoaded', function () {

    // ========================================
    // THEME TOGGLE (Light / Dark Mode)
    // ========================================
    const themeToggle = document.getElementById('themeToggle');
    const html = document.documentElement;

    const savedTheme = localStorage.getItem('cms-theme') || 'light';
    html.setAttribute('data-theme', savedTheme);
    updateThemeIcon(savedTheme);

    if (themeToggle) {
        themeToggle.addEventListener('click', () => {
            const current = html.getAttribute('data-theme');
            const next = current === 'dark' ? 'light' : 'dark';
            html.setAttribute('data-theme', next);
            localStorage.setItem('cms-theme', next);
            updateThemeIcon(next);
        });
    }

    function updateThemeIcon(theme) {
        if (!themeToggle) return;
        const icon = themeToggle.querySelector('i');
        if (icon) {
            icon.className = theme === 'dark' ? 'fas fa-sun' : 'fas fa-moon';
        }
    }

    // ========================================
    // SIDEBAR — Toggle & Mobile
    // ========================================
    const sidebar = document.getElementById('sidebar');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const mobileToggle = document.getElementById('mobileToggle');

    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener('click', () => {
            sidebar.classList.toggle('collapsed');
            document.querySelector('.main-content')?.classList.toggle('expanded');
        });
    }

    if (mobileToggle && sidebar) {
        mobileToggle.addEventListener('click', () => {
            sidebar.classList.toggle('open');
        });

        // Close sidebar when clicking outside on mobile
        document.addEventListener('click', (e) => {
            if (window.innerWidth <= 768 && sidebar.classList.contains('open')) {
                if (!sidebar.contains(e.target) && !mobileToggle.contains(e.target)) {
                    sidebar.classList.remove('open');
                }
            }
        });
    }

    // ========================================
    // TOAST NOTIFICATION — Auto dismiss
    // ========================================
    const toast = document.getElementById('toast');
    if (toast) {
        setTimeout(() => {
            toast.style.animation = 'fadeOut 0.4s forwards';
            setTimeout(() => toast.remove(), 400);
        }, 4500);
    }

    // ========================================
    // ANIMATED COUNTERS — Counter number class
    // ========================================
    const counters = document.querySelectorAll('.counter-number');
    if (counters.length > 0) {
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    animateCounter(entry.target);
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.5 });

        counters.forEach(counter => observer.observe(counter));
    }

    function animateCounter(element) {
        const target = parseInt(element.getAttribute('data-target')) || 0;
        const duration = 2000;
        const increment = target / (duration / 16);
        let current = 0;

        const timer = setInterval(() => {
            current += increment;
            if (current >= target) {
                element.textContent = target.toLocaleString() + (element.dataset.suffix || '');
                clearInterval(timer);
            } else {
                element.textContent = Math.floor(current).toLocaleString() + (element.dataset.suffix || '');
            }
        }, 16);
    }

    // ========================================
    // STAT VALUE COUNTER — data-count attribute
    // ========================================
    const statValues = document.querySelectorAll('.stat-value[data-count]');
    statValues.forEach(el => {
        const target = parseFloat(el.getAttribute('data-count'));
        const isDecimal = target % 1 !== 0;
        const suffix = el.dataset.suffix || '';
        const prefix = el.dataset.prefix || '';
        const duration = 1500;
        const startTime = performance.now();

        function update(currentTime) {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);
            // Ease-out cubic for smooth deceleration
            const eased = 1 - Math.pow(1 - progress, 3);
            const current = target * eased;

            el.textContent = prefix + (isDecimal ? current.toFixed(1) : Math.floor(current).toLocaleString()) + suffix;

            if (progress < 1) requestAnimationFrame(update);
        }

        requestAnimationFrame(update);
    });

    // ========================================
    // SVG PROGRESS RINGS — Circular progress
    // ========================================
    const progressRings = document.querySelectorAll('.progress-ring .progress');
    progressRings.forEach(ring => {
        const radius = ring.getAttribute('r');
        const circumference = 2 * Math.PI * radius;
        const percentage = ring.dataset.percentage || 0;

        ring.style.strokeDasharray = circumference;
        ring.style.strokeDashoffset = circumference;

        setTimeout(() => {
            const offset = circumference - (percentage / 100) * circumference;
            ring.style.strokeDashoffset = offset;
        }, 300);
    });

    // ========================================
    // STAGGER ANIMATION — Delay .animate-in cards
    // ========================================
    const animateInElements = document.querySelectorAll('.animate-in');
    animateInElements.forEach((el, index) => {
        el.style.animationDelay = `${index * 0.08}s`;
        el.style.opacity = '0';
        // Force reflow and set opacity back via animation
        requestAnimationFrame(() => {
            el.style.opacity = '';
        });
    });

    // ========================================
    // SCROLL REVEAL — .animate-on-scroll
    // ========================================
    const animateElements = document.querySelectorAll('.animate-on-scroll');
    if (animateElements.length > 0) {
        const scrollObserver = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('animate-in-view');
                    scrollObserver.unobserve(entry.target);
                }
            });
        }, { threshold: 0.1, rootMargin: '0px 0px -40px 0px' });

        animateElements.forEach((el, idx) => {
            el.style.transitionDelay = `${idx * 0.1}s`;
            scrollObserver.observe(el);
        });
    }

    // ========================================
    // CHART.JS INITIALIZATION
    // ========================================
    initCharts();

    // ========================================
    // MODAL SYSTEM — Open / Close
    // ========================================
    document.querySelectorAll('[data-modal]').forEach(btn => {
        btn.addEventListener('click', () => {
            const modal = document.getElementById(btn.dataset.modal);
            if (modal) modal.classList.add('active');
        });
    });

    document.querySelectorAll('.modal-close, .modal-overlay').forEach(el => {
        el.addEventListener('click', (e) => {
            if (e.target === el) {
                el.closest('.modal-overlay')?.classList.remove('active');
            }
        });
    });

    // ========================================
    // FORM VALIDATION — data-validate forms
    // ========================================
    document.querySelectorAll('form[data-validate]').forEach(form => {
        form.addEventListener('submit', (e) => {
            let isValid = true;
            form.querySelectorAll('[required]').forEach(input => {
                if (!input.value.trim()) {
                    isValid = false;
                    input.style.borderColor = 'var(--danger)';
                    input.style.boxShadow = '0 0 0 3px rgba(239, 68, 68, 0.2)';
                } else {
                    input.style.borderColor = '';
                    input.style.boxShadow = '';
                }
            });
            if (!isValid) e.preventDefault();
        });
    });

    // ========================================
    // SMOOTH SCROLL — Anchor links
    // ========================================
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const targetId = this.getAttribute('href');
            if (targetId === '#') return;
            const targetEl = document.querySelector(targetId);
            if (targetEl) {
                e.preventDefault();
                targetEl.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        });
    });

    // ========================================
    // PASSWORD VISIBILITY TOGGLE
    // ========================================
    document.querySelectorAll('.password-toggle').forEach(btn => {
        btn.addEventListener('click', function () {
            const wrapper = this.closest('.form-input-wrapper');
            const input = wrapper.querySelector('input');
            const icon = this.querySelector('i');

            if (input.type === 'password') {
                input.type = 'text';
                icon.className = 'fas fa-eye-slash';
            } else {
                input.type = 'password';
                icon.className = 'fas fa-eye';
            }
        });
    });

    // ========================================
    // TABLE SEARCH / FILTER
    // ========================================
    document.querySelectorAll('.table-search').forEach(searchInput => {
        searchInput.addEventListener('input', function () {
            const query = this.value.toLowerCase().trim();
            const tableContainer = this.closest('.card-body') || this.parentElement.parentElement;
            const table = tableContainer.querySelector('.data-table');
            if (!table) return;

            const rows = table.querySelectorAll('tbody tr');
            rows.forEach(row => {
                const text = row.textContent.toLowerCase();
                row.style.display = text.includes(query) ? '' : 'none';
            });
        });
    });

    // ========================================
    // BACK TO TOP BUTTON
    // ========================================
    const backToTopBtn = document.getElementById('backToTop');
    if (backToTopBtn) {
        window.addEventListener('scroll', () => {
            if (window.scrollY > 300) {
                backToTopBtn.classList.add('visible');
            } else {
                backToTopBtn.classList.remove('visible');
            }
        }, { passive: true });

        backToTopBtn.addEventListener('click', () => {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });
    }

    // ========================================
    // KEYBOARD SHORTCUTS
    // ========================================
    document.addEventListener('keydown', (e) => {
        // Escape closes modals
        if (e.key === 'Escape') {
            document.querySelectorAll('.modal-overlay.active').forEach(modal => {
                modal.classList.remove('active');
            });
            // Also close mobile sidebar
            if (sidebar && sidebar.classList.contains('open')) {
                sidebar.classList.remove('open');
            }
        }

        // Ctrl+K focuses search input
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            const searchInput = document.querySelector('.table-search');
            if (searchInput) {
                searchInput.focus();
                searchInput.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
        }
    });

    // ========================================
    // PAGE LOAD — Add animation class to body
    // ========================================
    document.body.classList.add('page-loading');

    // ========================================
    // FORM INPUT — Focus glow ring
    // ========================================
    document.querySelectorAll('.form-control').forEach(input => {
        input.addEventListener('focus', function () {
            this.closest('.form-group')?.classList.add('focused');
        });
        input.addEventListener('blur', function () {
            this.closest('.form-group')?.classList.remove('focused');
        });
    });

});

// ============================================
// CHART.JS — Initialize all charts
// ============================================
function initCharts() {
    if (typeof Chart !== 'undefined') {
        Chart.defaults.color = getComputedStyle(document.documentElement).getPropertyValue('--text-secondary').trim() || '#94a3b8';
        Chart.defaults.borderColor = getComputedStyle(document.documentElement).getPropertyValue('--border-color').trim() || 'rgba(255,255,255,0.08)';
        Chart.defaults.font.family = "'Inter', sans-serif";
    }

    // --- Department Doughnut Chart ---
    const deptCanvas = document.getElementById('departmentChart');
    if (deptCanvas) {
        const labels = JSON.parse(deptCanvas.dataset.labels || '[]');
        const values = JSON.parse(deptCanvas.dataset.values || '[]');

        new Chart(deptCanvas, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: values,
                    backgroundColor: ['#6366f1', '#8b5cf6', '#a78bfa', '#10b981', '#f59e0b', '#ef4444', '#3b82f6'],
                    borderWidth: 0,
                    hoverOffset: 8
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '65%',
                plugins: {
                    legend: { position: 'bottom', labels: { padding: 16, usePointStyle: true, pointStyleWidth: 10 } }
                }
            }
        });
    }

    // --- Attendance Line Chart ---
    const attendanceCanvas = document.getElementById('attendanceChart');
    if (attendanceCanvas) {
        const labels = JSON.parse(attendanceCanvas.dataset.labels || '[]');
        const values = JSON.parse(attendanceCanvas.dataset.values || '[]');

        new Chart(attendanceCanvas, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Attendance %',
                    data: values,
                    borderColor: '#6366f1',
                    backgroundColor: 'rgba(99, 102, 241, 0.1)',
                    fill: true,
                    tension: 0.4,
                    pointBackgroundColor: '#6366f1',
                    pointBorderColor: '#fff',
                    pointBorderWidth: 2,
                    pointRadius: 5,
                    pointHoverRadius: 8
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: { beginAtZero: true, max: 100, ticks: { callback: v => v + '%' }, grid: { color: 'rgba(255,255,255,0.04)' } },
                    x: { grid: { display: false } }
                },
                plugins: {
                    legend: { display: false }
                }
            }
        });
    }

    // --- Course Attendance Bar Chart ---
    const courseAttCanvas = document.getElementById('courseAttendanceChart');
    if (courseAttCanvas) {
        const labels = JSON.parse(courseAttCanvas.dataset.labels || '[]');
        const values = JSON.parse(courseAttCanvas.dataset.values || '[]');

        new Chart(courseAttCanvas, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Attendance %',
                    data: values,
                    backgroundColor: values.map(v => v >= 75 ? 'rgba(16, 185, 129, 0.7)' : v >= 50 ? 'rgba(245, 158, 11, 0.7)' : 'rgba(239, 68, 68, 0.7)'),
                    borderRadius: 6,
                    borderSkipped: false
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: { beginAtZero: true, max: 100, ticks: { callback: v => v + '%' }, grid: { color: 'rgba(255,255,255,0.04)' } },
                    x: { grid: { display: false } }
                },
                plugins: { legend: { display: false } }
            }
        });
    }

    // --- Fee Doughnut Chart ---
    const feeCanvas = document.getElementById('feeChart');
    if (feeCanvas) {
        const collected = parseFloat(feeCanvas.dataset.collected || 0);
        const pending = parseFloat(feeCanvas.dataset.pending || 0);

        new Chart(feeCanvas, {
            type: 'doughnut',
            data: {
                labels: ['Collected', 'Pending'],
                datasets: [{
                    data: [collected, pending],
                    backgroundColor: ['#10b981', '#ef4444'],
                    borderWidth: 0,
                    hoverOffset: 8
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '65%',
                plugins: {
                    legend: { position: 'bottom', labels: { padding: 16, usePointStyle: true } }
                }
            }
        });
    }
}

// ============================================
// UTILITY FUNCTIONS — Confirm, Modal helpers
// ============================================

/**
 * Shows a confirmation dialog for delete actions
 * @param {string} message - Custom confirmation message
 * @returns {boolean} - User's confirmation choice
 */
function confirmDelete(message) {
    return confirm(message || 'Are you sure you want to delete this item?');
}

/**
 * Shows a modal by its ID
 * @param {string} id - The modal element ID
 */
function showModal(id) {
    document.getElementById(id)?.classList.add('active');
}

/**
 * Hides a modal by its ID
 * @param {string} id - The modal element ID
 */
function hideModal(id) {
    document.getElementById(id)?.classList.remove('active');
}
