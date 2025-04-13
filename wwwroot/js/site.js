// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Bootstrap tooltip
document.addEventListener("DOMContentLoaded", function () {
    var tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');

    if (tooltipTriggerList.length > 0) {
        tooltipTriggerList.forEach(function (tooltipTriggerEl) {
            new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
});