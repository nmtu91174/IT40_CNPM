const modal = document.querySelector(".modal");
const modalLogin = document.querySelector(".modal__login");
const modalRegister = document.querySelector(".modal__register");
const openLoginModalBtn = document.querySelector("#btn-login");
const openRegisterModalBtn = document.querySelector("#btn-register");
const iconCloseLoginModal = document.querySelector(".i__login");
const iconCloseRegisterModal = document.querySelector(".i__register");
const buttonCloseLoginModal = document.querySelector(".btn__closeLogin");
const buttonCloseRegisterModal = document.querySelector(".btn__closeRegister");

//function toggleModal() {
//    modal.classList.toggle("hide");
//}
function toggleLoginModal() {
    modalLogin.classList.toggle("hide");
}
function toggleRegisterModal() {
    modalRegister.classList.toggle("hide");
}

openLoginModalBtn.addEventListener("click", toggleLoginModal);
openRegisterModalBtn.addEventListener("click", toggleRegisterModal);
iconCloseLoginModal.addEventListener("click", toggleLoginModal);
iconCloseRegisterModal.addEventListener("click", toggleRegisterModal);
buttonCloseLoginModal.addEventListener("click", toggleLoginModal);
buttonCloseRegisterModal.addEventListener("click", toggleRegisterModal);

modalLogin.addEventListener("click", (e) => {
    if (e.target == e.currentTarget) toggleLoginModal();
});
modalRegister.addEventListener("click", (e) => {
    if (e.target == e.currentTarget) toggleRegisterModal();
});