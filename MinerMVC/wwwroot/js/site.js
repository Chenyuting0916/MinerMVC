// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Theme toggle functionality
document.addEventListener('DOMContentLoaded', function() {
    // Theme toggle functionality
    const themeToggle = document.getElementById('themeToggle');
    const body = document.body;
    const moonIcon = '<i class="bx bx-moon"></i>';
    const sunIcon = '<i class="bx bx-sun"></i>';

    // Check saved theme
    const savedTheme = localStorage.getItem('theme') || 'dark';
    body.setAttribute('data-theme', savedTheme);
    themeToggle.innerHTML = savedTheme === 'dark' ? moonIcon : sunIcon;

    // Theme toggle click handler
    themeToggle.addEventListener('click', function() {
        const currentTheme = body.getAttribute('data-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        
        body.setAttribute('data-theme', newTheme);
        themeToggle.innerHTML = newTheme === 'dark' ? moonIcon : sunIcon;
        localStorage.setItem('theme', newTheme);
    });

    // Set active state for current page
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.activity-bar-icon');
    
    navLinks.forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.classList.add('active');
        }
    });

    // Modal functionality
    const modals = document.querySelectorAll('.modal');
    modals.forEach(modal => {
        modal.addEventListener('show.bs.modal', function() {
            const theme = body.getAttribute('data-theme');
            modal.querySelector('.modal-content').style.background = 
                theme === 'dark' ? '#1e1e1e' : '#ffffff';
            modal.querySelector('.modal-content').style.color = 
                theme === 'dark' ? '#e0e0e0' : '#000000';
        });
    });
});
