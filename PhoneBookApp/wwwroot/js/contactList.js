$(function () {
    loadContacts();


    //ajax za fetchanje svih drzava
    $.ajax({
        url: '/Contacts/GetCountries', // Endpoint za dohvat država
        type: 'GET',
        success: function (data) {
            let countryDropdown = $('#countryId');
            $.each(data, function (index, country) {
                countryDropdown.append($('<option>', {
                    value: country.value,
                    text: country.text
                }));
            });
        },
        error: function () {
            alert('Error fetching countries');
        }
    });

    let cityDropdown = $('#cityId');
    cityDropdown.prop('disabled', true); // Onemogućen dropdown Cities sve dok se ne odabere država
    cityDropdown.empty().append($('<option>', { value: '', text: 'Select country first' })); // Inicijalna prazna opcija dok se ne odabere država


    //na promjenu dropdowna Country fetchanje date za dropdown Cities
    $('#countryId').on('change', function () {
        let countryId = $(this).val();
        cityDropdown.empty(); // Očisti postojeće opcije gradova

        if (countryId) {
            $.ajax({
                url: '/Contacts/GetCitiesByCountry',
                type: 'GET',
                data: { countryId: countryId },
                success: function (data) {
                    cityDropdown.prop('disabled', false); // Omogući dropdown za odabir grada
                    cityDropdown.append($('<option>', { value: '', text: 'Select city', disabled: true, selected: true, hidden: true })); // Dodaj default opciju za odabir grada
                    $.each(data, function (index, city) {
                        cityDropdown.append($('<option>', {
                            value: city.value,
                            text: city.text
                        }));
                    });
                }
            });
        } else {
            cityDropdown.prop('disabled', true); // Ako nije odabrana država, onemogući polje
            cityDropdown.append($('<option>', { value: '', text: 'Select country' })); // Ponovno prikaži default opciju
        }
    });
});

//defaultno stanje za sort i paginaciju
let currentPage = 1;
let resultsPerPage = 5;
let sortColumn = "FirstName";
let sortOrder = "asc"; 

// Funkcija za dohvatanje svih kontakata
function loadContacts() {
    $('#loadingIndicator').show();
    $.ajax({
        url: '/Contacts/GetContacts',
        type: 'GET',
        dataType: 'json',
        data: {
            page: currentPage,
            resultsPerPage: resultsPerPage,
            sortColumn: sortColumn,
            sortOrder: sortOrder
        },
        success: function (data) {
            $('#loadingIndicator').hide();

            let tableBody = $('#contactsTable tbody');
            tableBody.empty();  // Očisti postojeće redove
            $.each(data.items, function (index, contact) {
                let genderIcon = contact.gender === "Male"
                    ? '<span class="gender-icon male"></span>'
                    : '<span class="gender-icon female"></span>';

                let row = `<tr id="contact-${contact.id}">
                        <td class="p-3 text-truncate" data-bs-toggle="tooltip" title="${contact.firstName}">${contact.firstName}</td>
                        <td class="p-3 text-truncate" data-bs-toggle="tooltip" title="${contact.lastName}">${contact.lastName}</td>
                        <td class="p-3 text-truncate">${contact.phoneNumber}</td>
                        <td class="p-3 text-truncate">${genderIcon}</td>
                        <td class="p-3 text-truncate" data-bs-toggle="tooltip" title="${contact.email}">${contact.email}</td>
                        <td class="p-3 text-truncate">${contact.birthDate}</td>
                        <td class="p-3 text-truncate">${contact.age}</td>
                        <td class="p-3 text-truncate">${contact.city}</td>
                        <td class="p-3 text-truncate">${contact.country}</td>
                        <td class="p-auto w-15 text-truncate">
                            <button class="btn btn-info shadow" onclick="editContactModal(${contact.id})">Edit</button>
                            <button class="btn btn-danger shadow" onclick="openDeleteModal(${contact.id}, '${contact.firstName}', '${contact.lastName}')">Delete</button>
                        </td>
                    </tr>`;

                tableBody.append(row);
            });
            // Update pagination buttons
            updatePaginationButtons(data.totalPages);
        },
        error: function () {
            alert("Error loading contacts.");
        }
    });
}

// Funkcija za sortiranje rezultata po kolonama ima i prezime
function sortTable(column) {
    if (sortColumn === column) {
        // Ako je ista kolona, menjamo redosled (asc/desc)
        sortOrder = (sortOrder === "asc") ? "desc" : "asc";
    } else {
        // Ako je druga kolona, postavljamo sortiranje na asc
        sortColumn = column;
        sortOrder = "asc";
    }
    loadContacts();  // Ponovno učitaj kontakte sa novim parametrima

    // Promeni strelicu u zavisnosti od sortiranja
    updateArrowIcons();
}

// Funkcija za ažuriranje strelica asc/desc
function updateArrowIcons() {
    // Resetuj sve strelice
    $('#firstNameArrow').removeClass('asc desc');
    $('#lastNameArrow').removeClass('asc desc');

    // Dodaj odgovarajuću klasu za strelicu
    if (sortColumn === 'FirstName') {
        $('#firstNameArrow').addClass(sortOrder);
    } else if (sortColumn === 'LastName') {
        $('#lastNameArrow').addClass(sortOrder);
    }
}

//Update buttona za paginaciju
function updatePaginationButtons(totalPages) {
    $('#prevPage').prop('disabled', currentPage === 1);
    $('#nextPage').prop('disabled', currentPage === totalPages);
    $('#currentPage').text(`Page ${currentPage}`);
}

// Prethodna stranica
$('#prevPage').on("click", function () {
    if (currentPage > 1) {
        currentPage--;
        loadContacts();
    }
});

// Sljedeća stranica
$('#nextPage').on("click", function () {
    currentPage++;
    loadContacts();
});

// loaduje rezultate onchange dropdowna
$('#resultsPerPage').on("change", function () {
    resultsPerPage = parseInt($(this).val());
    currentPage = 1;  // Resetuje na prvu stranu
    loadContacts();
});


// Funkcija za otvaranje modala za kreiranje contacta "Add new contact"
function openCreateModal() {
    $('#createContactForm').trigger('reset'); //resetovanje forme
    $('#createContactModal').modal('show'); // Otvara modal
    $('#headerColor').removeClass('bg-info').addClass('bg-primary'); //postavlja željenu boju u header
    $('#createContactModalLabel').text('Add new contact');// Postavlja naslov u headeru na "Add new contact"
    $('#saveButton').text('Save').attr('onclick', `createContact()`); //dodaje text i funkcionalnost buttona

    let cityDropdown = $('#cityId');
    cityDropdown.prop('disabled', true); // Onemogućen dropdown Cities sve dok se ne odabere država
    cityDropdown.empty().append($('<option>', { value: '', text: 'Select country first' })); // Inicijalna prazna opcija dok se ne odabere država
}

// Funkcija za otvaranje modala za brisanje
function openDeleteModal(contactId, firstName, lastName) {
    $('#contactNameToDelete').text(firstName + ' ' + lastName); // Postavlja ime i prezime kontakta u modal
    $('#deleteContactModal').data('contactId', contactId); // Sprema ID kontakta u data atribut modala
    $('#deleteContactModal').modal('show'); // Otvorite modal
}

// Funkcija za brisanje kontakta
function deletingFn() {
    let contactId = $('#deleteContactModal').data('contactId');
    if (contactId) {
        // Pošaljite AJAX zahtjev za brisanje kontakta
        $.ajax({
            url: '/contacts/delete/' + contactId,
            type: 'DELETE',
            success: function (response) {
                if (response.success) {
                    showToast(response.message, 'success');
                    $('#deleteContactModal').modal('hide');
                    // Osvježavanje liste kontakata iz DOM-a
                    $('#contact-' + contactId).remove();
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert('Došlo je do greške pri komunikaciji sa serverom.');
            }
        });
        // Sakrijte modal
        $('#deleteContactModal').modal('hide');
    }
};




// Funkcija za slanje podataka kada korisnik klikne "Save"
function createContact() {
    // Provjera validnosti podataka
    if (!isValidInput()) {
        return;
    }

    let birthDateVal = $('#birthDate').val();
    let formattedBirthDate = new Date(birthDateVal).toISOString();

    // Nastavak slanja podataka
    const contactData = {
        firstName: $('#firstName').val().trim(),
        lastName: $('#lastName').val().trim(),
        phoneNumber: $('#phoneNumber').val().trim(),
        email: $('#email').val().trim(),
        birthDate: formattedBirthDate,
        gender: parseInt($('#gender').val(), 10),
        cityId: parseInt($('#cityId').val(), 10),
        countryId: parseInt($('#countryId').val(), 10)
    };

    // AJAX za snimanje podataka
    $.ajax({
        url: '/Contacts/Create',
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify(contactData),
        contentType: 'application/json',
        success: function (response) {
            if (response.message) {
                $('#createContactModal').modal('hide');
                $('#createContactForm').trigger('reset');
                showToast(response.message, 'success');
                loadContacts();
            } else {
                alert('Došlo je do greške pri kreiranju kontakta.');
            }
        },
        error: function () {
            alert('Došlo je do greške pri slanju podataka.');
        }
    });
}


//funkcija za editovanje kontakta
function editContactModal(contactId) {
    $('#createContactModalLabel').text('Edit Contact'); // Postavlja naslov u headeru na "Save edited"
    $('#headerColor').removeClass('bg-primary').addClass('bg-info'); //postavlja željenu biju u header
    $('#saveButton').text('Save Edited').attr('onclick', `saveEditedContact(${contactId})`); // Postavlja text "Save edited" i funkcionalnost buttona

    // AJAX poziv za dohvat podataka kontakta prema ID-u
    $.ajax({
        url: '/contacts/getContactById/' + contactId,  // Endpoint za dohvat kontakta po ID-u
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                // Popunjavanje modalnih polja s podacima kontakta
                $('#firstName').val(response.contact.firstName);
                $('#lastName').val(response.contact.lastName);
                $('#phoneNumber').val(response.contact.phoneNumber);
                $('#gender').val(response.contact.gender);
                $('#email').val(response.contact.email);
                $('#birthDate').val(response.contact.birthDate.split('T')[0]);
                $('#countryId').val(response.contact.countryId);

                // Popunjavanje dropdowna za gradove
                let cityDropdown = $('#cityId').prop('disabled', false);
                cityDropdown.empty(); // Očisti postojeće gradove

                // Dodaj gradove za odabranu državu
                $.each(response.cities, function (index, city) {
                    let option = $('<option>', {
                        value: city.id,
                        text: city.name
                    });

                    // Selektuj grad koji je vezan za korisnika
                    if (city.id === response.contact.cityId) {
                        option.prop('selected', true);
                    }
                    cityDropdown.append(option);
                });

                // Otvaranje modal za edit
                $('#createContactModal').modal('show');
            } else {
                alert('Error fetcing contact data.');
            }
        },
        error: function () {
            alert("Error fetching contact data.");
        }
    });
}


// Funkcija za snimanje izmjena editovanog kontakta
function saveEditedContact(id) {

    // Provjera validnosti podataka
    if (!isValidInput()) {
        return;
    }

    let birthDateVal = $('#birthDate').val();
    let formattedBirthDate = new Date(birthDateVal).toISOString();

    // Priprema podataka za slanje
    const contactData = {
        id: id, // ID kontakta koji se uređuje
        firstName: $('#firstName').val(),
        lastName: $('#lastName').val(),
        phoneNumber: $('#phoneNumber').val(),
        email: $('#email').val(),
        birthDate: formattedBirthDate,
        gender: parseInt($('#gender').val(), 10),
        cityId: parseInt($('#cityId').val(), 10),
        countryId: parseInt($('#countryId').val(), 10)
    };

    $.ajax({
        url: `/contacts/update/${id}`,  // ID kontakta sada ide u URL
        type: 'PUT',
        dataType: 'json',
        data: JSON.stringify(contactData),
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                $('#createContactModal').modal('hide');  // Zatvori modal
                $('#createContactForm').trigger('reset');  // Resetiraj formu
                showToast(response.message, 'success');  // Prikaži toast
                loadContacts();  // Osvježi listu kontakata
            } else {
                alert('Došlo je do greške pri ažuriranju kontakta.');
            }
        },
        error: function () {
            alert('Došlo je do greške pri slanju podataka.');
        }
    });
}




//Prikazivanje notifikacija - tost
function showToast(message, type = 'success') {
    let toast = new bootstrap.Toast(document.getElementById('toastNotification'));
    let toastBody = document.getElementById('toastMessage');

    // Postavi poruku u toast
    toastBody.innerText = message;

    // Postavi boju (za error/crveni notifikator koristi danger klasu)
    let toastElement = document.getElementById('toastNotification');
    toastElement.className = `toast align-items-center text-white bg-${type} border-0`;

    // Prikaži toast
    toast.show();
}


// Funkcija za validaciju podataka
function isValidInput() {
    let firstName = $('#firstName').val().trim();
    let lastName = $('#lastName').val().trim();
    let phoneNumber = $('#phoneNumber').val().trim();
    let email = $('#email').val().trim();
    let birthDate = $('#birthDate').val().trim();
    let gender = $('#gender').val();
    let cityId = $('#cityId').val();
    let countryId = $('#countryId').val();

    // Regularni izrazi za validaciju
    let nameRegex = /^[A-Za-zčćžšđČĆŽŠĐ ]+$/; // Samo slova i razmaci
    let phoneRegex = /^[0-9]{3}\/[0-9]{3}-[0-9]{3}$/; // Format: 062/415-654
    let emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/; // Standardna email validacija

    // Provjera da polja nisu prazna
    if (!firstName || !lastName || !phoneNumber || !email || !birthDate || !gender || !cityId || !countryId) {
        alert('Please fill in all fields!');
        return false;
    }

    // Validacija imena i prezimena
    if (!nameRegex.test(firstName)) {
        alert('First name can only contain letters!');
        return false;
    }
    if (!nameRegex.test(lastName)) {
        alert('Last name can only contain letters!');
        return false;
    }

    // Validacija dužine imena i prezimena (max 50 znakova)
    if (firstName.length > 50) {
        alert('First name must be less than or equal to 50 characters!');
        return false;
    }
    if (lastName.length > 50) {
        alert('Last name must be less than or equal to 50 characters!');
        return false;
    }

    // Validacija broja telefona
    if (!phoneRegex.test(phoneNumber)) {
        alert('Phone number must be in the format XXX/XXX-XXX !');
        return false;
    }

    // Validacija emaila
    if (!emailRegex.test(email)) {
        alert('Please enter a valid email address!');
        return false;
    }

    // Validacija datuma rođenja
    let today = new Date();
    let birthDateObj = new Date(birthDate);
    let minBirthDate = new Date();
    minBirthDate.setFullYear(today.getFullYear() - 100); // Prije 100 godina

    if (birthDateObj > today) {
        alert('Birth date cannot be in the future!');
        return false;
    }
    if (birthDateObj < minBirthDate) {
        alert('Birth date is not valid!');
        return false;
    }

    return true;
}


//Validacije ispod se izvršavaju na pojedinačna polja gdje korisnik tokom unosa u polje instantno dobija feedback o validnosti unosa

//validiranje broja telefona u toku unosa u input polje phoneNumber
$(document).on("input", "#phoneNumber", function () {
    let input = $(this).val().replace(/\D/g, '');
    let formatted = '';

    if (input.length > 3) {
        formatted += input.substring(0, 3) + '/';
        if (input.length > 6) {
            formatted += input.substring(3, 6) + '-';
            formatted += input.substring(6, 9);
        } else {
            formatted += input.substring(3, 6);
        }
    } else {
        formatted = input;
    }

    $(this).val(formatted);
});

//validacija polja firstName i lastName u toku unosa u polje za firstName i lastName
$(document).on("input focusout", "#firstName, #lastName", function () {
    let nameRegex = /^[A-Za-zčćžšđČĆŽŠĐ ]+$/; // Dozvoljena su samo slova i razmaci
    let maxLength = 50; // Maksimalna dužina
    let minLength = 1; // Minimalna dužina
    let value = $(this).val().trim(); // Uklanja početne/završne razmake
    !nameRegex.test(value)
    if (!value) {
        $(this).addClass("is-invalid"); // Dodaje crveni border
        $(this).siblings(".invalid-feedback").text("Field cannot be empty!").show();
    } else if (value.length > maxLength) {
        $(this).val(value.substring(0, maxLength)); // Skraćuje unos
        $(this).addClass("is-invalid");
        $(this).siblings(".invalid-feedback").text("Maximum 50 characters allowed.").show();
    } else if (!nameRegex.test(value)) {
        $(this).addClass("is-invalid");
        $(this).siblings(".invalid-feedback").text("Only letters and spaces are allowed!").show();
    } else {
        $(this).removeClass("is-invalid").addClass("is-valid"); // Skida crveni border
        $(this).siblings(".invalid-feedback").text("").hide(); // Sakriva poruku
    }
});

//validacija za input u polje phoneNumber
$(document).on("input focusout", "#phoneNumber", function () {
    let phoneRegex = /^[0-9]{3}\/[0-9]{3}-[0-9]{3}$/; // Format: XXX/XXX-XXX
    let value = $(this).val().trim();

    if (!value) {
        $(this).addClass("is-invalid"); // Dodaje crveni border
        $(this).siblings(".invalid-feedback").text("Phone number can not be empty!").show();
    } else if(!phoneRegex.test(value)){
        $(this).addClass("is-invalid"); // Dodaje crveni border
        $(this).siblings(".invalid-feedback").text("Phone number must be in format XXX/XXX-XXX").show();
    } else {
        $(this).removeClass("is-invalid").addClass("is-valid"); // Skida crveni border
        $(this).siblings(".invalid-feedback").text("").hide(); // Sakriva poruku
    }
});

//validacija za input u polje email
$(document).on("input focusout", "#email", function () {
    let emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/; // Standardna email validacija
    let value = $(this).val().trim();
    if (!value) {
        $(this).addClass("is-invalid"); // Dodaje crveni border
        $(this).siblings(".invalid-feedback").text("Field can not be empty!").show();
    } else if (!emailRegex.test(value)) {
        $(this).addClass("is-invalid"); // Dodaje crveni border
        $(this).siblings(".invalid-feedback").text("Invalid email format!").show();
    } else {
        $(this).removeClass("is-invalid").addClass("is-valid"); // Skida crveni border
        $(this).siblings(".invalid-feedback").text("").hide(); // Sakriva poruku
    }
});

// On input validacija za datum rođenja
$(document).on("input focusout", "#birthDate", function () {
    let birthDate = $(this).val().trim(); // Uzima uneseni datum
    let today = new Date();
    let birthDateObj = new Date(birthDate);
    let minBirthDate = new Date();
    minBirthDate.setFullYear(today.getFullYear() - 100); // Prije 100 godina

    // Provjera da datum nije u budućnosti
    if (birthDateObj > today) {
        $(this).addClass("is-invalid");
        $(this).siblings(".invalid-feedback").text("Birth date cannot be in the future!").show();
        return false;
    }

    // Provjera da datum nije stariji od 100 godina
    if (birthDateObj < minBirthDate) {
        $(this).addClass("is-invalid");
        $(this).siblings(".invalid-feedback").text("Birth date can not be older than 100 years!").show();
        return false;
    }

    // Provjera da li je uopšte unešen
    if (!birthDate) {
        $(this).addClass("is-invalid");
        $(this).siblings(".invalid-feedback").text("Birth date is required!").show();
        return false;
    }

    // Ako je datum validan, ukloni crveni border i sakrij poruku
    $(this).removeClass("is-invalid").addClass("is-valid");
    $(this).siblings(".invalid-feedback").text("").hide();
    return true;
});

// on input validacija da korisnik mora odabrati jedan spol
$("#gender").on("input focusout", function () {
    let feedbackElement = $(this).parent().find(".invalid-feedback");

    if ($(this).val() === null || $(this).val() === "") {
        $(this).addClass("is-invalid");
        feedbackElement.text("You must choose gender.").show();
    } else {
        $(this).removeClass("is-invalid").addClass("is-valid");
        feedbackElement.text("").hide();
    }
});

// Validacija za country
$("#countryId").on("input focusout", function () {
    let feedbackElement = $(this).parent().find(".invalid-feedback");

    if ($(this).val() === null || $(this).val() === "") {
        $(this).addClass("is-invalid");
        feedbackElement.text("You must choose country.").show();
    } else {
        $(this).removeClass("is-invalid").addClass("is-valid");
        feedbackElement.text("").hide();
    }
});

// Validacija za city
$("#cityId").on("input focusout", function () {
    let feedbackElement = $(this).parent().find(".invalid-feedback");

    if ($(this).val() === null || $(this).val() === "") {
        $(this).addClass("is-invalid");
        feedbackElement.text("You must choose city.").show();
    } else {
        $(this).removeClass("is-invalid").addClass("is-valid");
        feedbackElement.text("").hide();
    }
});

//uklanjanje poruka validacije na zatvaranju modala
$('#createContactModal').on('hidden.bs.modal', function () {
    // Uklanja crveni border sa svih input polja
    $(this).find(".is-invalid").removeClass("is-invalid");
    $(this).find(".is-valid").removeClass("is-valid");

    // Sakriva sve poruke greške
    $(this).find(".invalid-feedback").text("").hide();
});
