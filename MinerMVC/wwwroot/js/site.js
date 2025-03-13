// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Theme toggle functionality
document.addEventListener('DOMContentLoaded', function() {
    const themeToggle = document.getElementById('themeToggle');
    const themeIcon = themeToggle.querySelector('i');
    const body = document.documentElement;
    const moonIcon = 'bx bx-moon';
    const sunIcon = 'bx bx-sun';

    // 從 localStorage 獲取主題設置
    const savedTheme = localStorage.getItem('theme') || 'dark';
    body.setAttribute('data-theme', savedTheme);
    themeIcon.className = savedTheme === 'dark' ? moonIcon : sunIcon;

    // 主題切換事件
    themeToggle.addEventListener('click', () => {
        const currentTheme = body.getAttribute('data-theme');
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        
        body.setAttribute('data-theme', newTheme);
        themeIcon.className = newTheme === 'dark' ? moonIcon : sunIcon;
        localStorage.setItem('theme', newTheme);

        // 更新模態框背景顏色
        const modals = document.querySelectorAll('.modal-content');
        modals.forEach(modal => {
            modal.style.backgroundColor = getComputedStyle(document.documentElement)
                .getPropertyValue(newTheme === 'dark' ? '--vscode-bg' : '--vscode-bg').trim();
        });
    });

    // 設置當前頁面的導航鏈接為活動狀態
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.activity-bar-icon');
    navLinks.forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.classList.add('active');
        }
    });
});
