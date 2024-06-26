const main = document.querySelector("main");

// Gestion pop up register
const openPopup = () => {
    let popup = document.getElementById("popupRole");
    popup.style.display = "block";
    main.style.filter = 'blur(5px)';
    popup.style.boxShadow = "rgba(9, 30, 66, 0.25) 0px 4px 8px -2px, rgba(9, 30, 66, 0.08) 0px 0px 0px 1px";
};

const closePopup = () => {
    let popup = document.getElementById("popupRole");
    popup.style.display = "none";
    main.style.filter = 'blur(0px)';
};

const closePopupRole = document.querySelector("#closePopupRole");
const registerPopUp = document.querySelector("#registerPopUp");
const registerBtn = document.querySelector("#registerBtn");

if (registerPopUp) {
    registerPopUp.addEventListener("click", openPopup);
}

if (registerBtn) {
    registerBtn.addEventListener("click", openPopup);
}

if (closePopupRole) {
    closePopupRole.addEventListener("click", closePopup);
}



//GESTION DE LA POP-UP DU PROFILE
const profileBtn = document.querySelector("#profileBtn");
const closePopupProfile = document.querySelector("#closePopupProfile");
if(profileBtn != null && closePopupProfile != null){
    profileBtn.addEventListener("click", () => {
        let popup = document.getElementById("popupProfile");
        popup.style.display = "block";
        main.style.filter = 'blur(5px)';
        popup.style.boxShadow = "rgba(9, 30, 66, 0.25) 0px 4px 8px -2px, rgba(9, 30, 66, 0.08) 0px 0px 0px 1px";
    });

    closePopupProfile.addEventListener("click", () => {
        let popup = document.getElementById("popupProfile");
        popup.style.display = "none";
        main.style.filter = 'blur(0px)';
    });
}


//GESTION DE LA SELECTION D'IMAGE
function previewImage(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function(e) {
            $('#previewImage').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    }
}

// Appeler la fonction previewImage lorsque le champ de sélection de fichier change
$('#inputFile').change(function() {
    previewImage(this);
});



//GESTION DE L'AFFICHE REGISTER AVEC LES DIFFERENTS PERMIS
const permisB = document.getElementById("permisB");
const permisC = document.getElementById("permisC");
const permisCE = document.getElementById("permisCE");
const roleMember = document.getElementById("roleMember");
const levelOfStudy = document.getElementById("levelOfStudy");

if(permisB) {
    permisB.addEventListener("change", updateLevelOfStudyVisibility);
}
if(permisC) {
    permisC.addEventListener("change", updateLevelOfStudyVisibility);
}
if(permisCE) {
    permisCE.addEventListener("change", updateLevelOfStudyVisibility);
}

function updateLevelOfStudyVisibility() {
    if (permisB.checked && !permisC.checked && !permisCE.checked) {
        levelOfStudy.style.display = "block";
        roleMember.innerText = "dispatcher"
    } else {
        levelOfStudy.style.display = "none";
        roleMember.innerText = "chauffeur"
    }
}

