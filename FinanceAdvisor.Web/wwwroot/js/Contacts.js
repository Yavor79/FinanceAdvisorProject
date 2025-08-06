<script>
    const sideNav = document.getElementById('sideNav');
    const contactBtn = document.getElementById('contactBtn');
    const closeBtn = document.getElementById('closeBtn');

    contactBtn.addEventListener('click', () => {
        sideNav.style.width = '300px'; // ширина на навигационното поле
    });

    closeBtn.addEventListener('click', () => {
        sideNav.style.width = '0';
    });
</script>