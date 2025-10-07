
// add to cart function
window.addToCart = async function (partId, partName, partPrice) {
    const data = {
        id: partId,
        name: partName,
        price: partPrice
    };
  try {
        const response = await fetch('/Cart/AddToCart', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        if (!response.ok) throw new Error(`Server error: ${response.status}`);      

        const result = await response.json();

        if (result.success) {           
            showToast(result.message, "success");
            await loadCart();
        } else {           
            showToast(result.message, "error");
        }

    } catch (err) {        
        showToast("Network error occurred", "error");
    }
};
   
