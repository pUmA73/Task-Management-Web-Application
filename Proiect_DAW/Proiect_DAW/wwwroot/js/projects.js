document.addEventListener('DOMContentLoaded', function () {
    var overlay = document.querySelector('.overlay');
    var halfCircleButton = document.querySelector('.half-circle-button');
    var arrowIcon = document.querySelector('.arrow-icon');
    halfCircleButton.addEventListener('click', function () {
        // Toggle the class on the arrow icon
        arrowIcon.classList.toggle('rotate-arrow');
    });
    halfCircleButton.addEventListener('click', function () {
        flip_arrow();
    });

    function flip_arrow() {
        document.querySelector('.arrow-icon').classList.toggle('rotate-arrow');
    }
    var nav_toggle = false;
    document.getElementById('arrow').addEventListener('click', function () {
        if (nav_toggle == false) {
            this.style.left = (this.offsetLeft + 220) + 'px';
            nav_toggle = true;
        }
        else {
            this.style.left = (this.offsetLeft - 220) + 'px';
            nav_toggle = false;
        }
    });
    var offcanvasToggleElements = document.querySelectorAll('[data-toggle="offcanvas"]');
    offcanvasToggleElements.forEach(function (elem) {
        elem.addEventListener('click', function () {
            var wrapper = document.getElementById('wrapper');
            wrapper.classList.toggle('toggled');
        });
    });


});