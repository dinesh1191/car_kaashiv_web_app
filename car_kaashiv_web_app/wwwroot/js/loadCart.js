

window.loadCart = async function () {
    const cartContent = document.getElementById("cartContent");
    const cartModal = new bootstrap.Modal(document.getElementById("cartModal"));
    try {
        const response = await fetch("/Cart/GetCartPartial");

        if (!response.ok) {
            throw new Error("Network response was not ok");
        }
        const html = await response.text();
        cartContent.innerHTML = html;
        // Show the modal using Bootstrap API
        const cartModalEl = document.getElementById("cartModal");
        const cartModal = bootstrap.Modal.getOrCreateInstance(cartModalEl);
        cartModal.show();
    } catch (error) {
        cartContent.innerHTML = "Error loading cart. Please try again.";
        console.error("Fetch error:", error);
        showToast("Unable to load cart", "error");
    }
};

