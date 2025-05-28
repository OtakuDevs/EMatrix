// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let cart = JSON.parse(localStorage.getItem('cart')) || [];

function fetchCartAndPreview(){
    fetch('/Cart/ReceiveCartContent', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(cart)
    })
        .then(response => {
            if (response.ok) {
                window.location.href = response.url.replace(/ReceiveCartContent$/, "CartPreview");
            } else {
                return response.text().then(text => console.error("Response:", text));
            }
        })
        .catch(error => console.error("Error:", error));
}