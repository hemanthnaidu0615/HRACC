document.addEventListener('DOMContentLoaded', function () {
    var emailInput = document.getElementById('EmployeeEmail');
    var emailError = document.getElementById('emailError');

    console.log("Validating email");

    // Regex for validating email
    var emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9-]+\.(com|org|net|edu|gov|mil|biz|info)$/;

    emailInput.addEventListener('input', function () {
        var email = emailInput.value.trim();

        // Show error if email is non-empty and does NOT match regex
        if (email.length > 0 && !emailRegex.test(email)) {
            emailError.style.display = 'inline';
            emailError.textContent = 'Please enter a valid email address (e.g., example@gmail.com)';
        } else {
            emailError.style.display = 'none';
        }
    });
});
