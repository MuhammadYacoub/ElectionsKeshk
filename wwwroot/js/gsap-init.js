/**
 * GSAP Animation System for AdvancedVotingSystem
 * Professional institutional animations
 */

document.addEventListener('DOMContentLoaded', function () {
    // Ensure GSAP is loaded
    if (typeof gsap === 'undefined') return;

    // ---- Page Load Animations ---- //

    // Animate sidebar nav items
    gsap.from('.sidebar-nav-item', {
        x: 30,
        opacity: 0,
        duration: 0.4,
        stagger: 0.05,
        ease: 'power2.out',
        delay: 0.2
    });

    // Animate page header
    gsap.from('.page-header', {
        y: -20,
        opacity: 0,
        duration: 0.5,
        ease: 'power2.out',
        delay: 0.1
    });

    // Animate statistic cards with stagger
    gsap.from('.ant-statistic', {
        y: 30,
        opacity: 0,
        duration: 0.6,
        stagger: 0.1,
        ease: 'power3.out',
        delay: 0.2
    });

    // Animate cards with stagger
    gsap.from('.ant-card', {
        y: 20,
        opacity: 0,
        duration: 0.5,
        stagger: 0.08,
        ease: 'power2.out',
        delay: 0.3
    });

    // Animate table rows
    gsap.from('.ant-table tbody tr', {
        x: 20,
        opacity: 0,
        duration: 0.4,
        stagger: 0.03,
        ease: 'power2.out',
        delay: 0.3
    });

    // Animate buttons in page header
    gsap.from('.page-header .ant-btn', {
        scale: 0.9,
        opacity: 0,
        duration: 0.4,
        stagger: 0.1,
        ease: 'back.out(1.7)',
        delay: 0.4
    });

    // Animate alert messages
    gsap.from('.ant-alert', {
        y: -10,
        opacity: 0,
        duration: 0.5,
        ease: 'power2.out'
    });

    // ---- GSAP Utility Functions ---- //

    // CountUp animation for statistic values
    window.gsapCountUp = function (element, endValue, duration) {
        if (!element) return;
        const obj = { val: 0 };
        gsap.to(obj, {
            val: endValue,
            duration: duration || 2,
            ease: 'power2.out',
            onUpdate: function () {
                element.textContent = Math.round(obj.val).toLocaleString('ar-EG');
            }
        });
    };

    // Initialize countup for all stat values
    document.querySelectorAll('[data-countup]').forEach(function (el) {
        const value = parseInt(el.getAttribute('data-countup'));
        if (!isNaN(value)) {
            el.textContent = '0';
            gsapCountUp(el, value, 1.5);
        }
    });

    // ---- Modal Animations ---- //

    window.gsapOpenModal = function (modalId) {
        const modal = document.getElementById(modalId);
        if (!modal) return;

        modal.classList.remove('hidden');
        modal.style.display = 'flex';

        const overlay = modal;
        const content = modal.querySelector('.ant-modal') || modal.children[0];

        gsap.fromTo(overlay, { opacity: 0 }, { opacity: 1, duration: 0.3, ease: 'power2.out' });
        gsap.fromTo(content,
            { scale: 0.9, opacity: 0, y: 20 },
            { scale: 1, opacity: 1, y: 0, duration: 0.4, ease: 'back.out(1.4)', delay: 0.1 }
        );
    };

    window.gsapCloseModal = function (modalId, callback) {
        const modal = document.getElementById(modalId);
        if (!modal) return;

        const content = modal.querySelector('.ant-modal') || modal.children[0];

        gsap.to(content, { scale: 0.95, opacity: 0, y: 10, duration: 0.2, ease: 'power2.in' });
        gsap.to(modal, {
            opacity: 0, duration: 0.3, ease: 'power2.in', delay: 0.1,
            onComplete: function () {
                modal.classList.add('hidden');
                modal.style.display = 'none';
                if (callback) callback();
            }
        });
    };

    // ---- Hover Effects ---- //

    // Card hover lift effect
    document.querySelectorAll('.ant-card, .ant-statistic').forEach(function (card) {
        card.addEventListener('mouseenter', function () {
            gsap.to(card, { y: -2, duration: 0.2, ease: 'power2.out' });
        });
        card.addEventListener('mouseleave', function () {
            gsap.to(card, { y: 0, duration: 0.2, ease: 'power2.out' });
        });
    });

    // ---- GSAP Fade In for elements with class ---- //
    document.querySelectorAll('.gsap-fade-in').forEach(function (el) {
        gsap.to(el, { opacity: 1, duration: 0.6, ease: 'power2.out', delay: 0.2 });
    });

    document.querySelectorAll('.gsap-slide-up').forEach(function (el) {
        gsap.to(el, { opacity: 1, y: 0, duration: 0.6, ease: 'power3.out', delay: 0.3 });
    });

    document.querySelectorAll('.gsap-slide-right').forEach(function (el) {
        gsap.to(el, { opacity: 1, x: 0, duration: 0.5, ease: 'power2.out', delay: 0.2 });
    });

    document.querySelectorAll('.gsap-scale-in').forEach(function (el) {
        gsap.to(el, { opacity: 1, scale: 1, duration: 0.5, ease: 'back.out(1.7)', delay: 0.3 });
    });

    // Stagger children
    document.querySelectorAll('.gsap-stagger-parent').forEach(function (parent) {
        gsap.from(parent.children, {
            y: 20,
            opacity: 0,
            duration: 0.4,
            stagger: 0.06,
            ease: 'power2.out',
            delay: 0.2
        });
    });

    // ---- Data Attribute Animations ---- //
    document.querySelectorAll('[data-gsap="fade-up"]').forEach(function (el, index) {
        gsap.from(el, {
            y: 30,
            opacity: 0,
            duration: 0.7,
            ease: 'power3.out',
            delay: 0.15 * index + 0.2
        });
    });

    document.querySelectorAll('[data-gsap="fade-in"]').forEach(function (el, index) {
        gsap.from(el, {
            opacity: 0,
            duration: 0.6,
            ease: 'power2.out',
            delay: 0.1 * index + 0.2
        });
    });
});
