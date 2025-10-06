
// add to cart function
    window.addToCart = function(partId, partName, partPrice){
        const data = {
            id: partId,
            name: partName,
            price: partPrice
        };
        console.log("Sending to server:", data);
        fetch('/Cart/AddToCart', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
            .then(res => res.json())
            .then(response => {
                if (response.success) {
                    console.log("console print is success", response.success);
                    showAlertModal("Added to cart!", "success");
                } else {
                    showAlertModal(response.message, "error");
                }
            }).catch(err => console.error("Fetch error:", err));
    }   
