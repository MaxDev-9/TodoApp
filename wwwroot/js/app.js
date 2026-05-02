// Smooth page transitions and micro interactions
document.addEventListener('DOMContentLoaded', () => {
    // Auto-dismiss alerts
    document.querySelectorAll('.alert').forEach(alert => {
        setTimeout(() => {
            alert.style.transition = 'opacity 0.5s ease';
            alert.style.opacity = '0';
            setTimeout(() => alert.remove(), 500);
        }, 4000);
    });

    // Active nav link highlighting
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.nav-links a').forEach(link => {
        const href = link.getAttribute('href')?.toLowerCase();
        if (href && currentPath.startsWith(href) && href !== '/') {
            link.classList.add('active');
        }
    });

    // Animate elements on scroll
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, { threshold: 0.1 });

    document.querySelectorAll('.glass, .list-card, .task-item, .stat-card').forEach(el => {
        if (!el.style.animation) {
            el.style.transition = 'opacity 0.4s ease, transform 0.4s ease';
            observer.observe(el);
        }
    });
});
