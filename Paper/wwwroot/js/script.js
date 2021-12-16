

let arrow = document.querySelectorAll(".arrow");
let menu = document.querySelectorAll(".menu_show");
for (var i = 0; i < arrow.length; i++) {
    arrow[i].addEventListener("click", (e)=>{
        let arrowParent = e.target.parentElement.parentElement;//selecting main parent of arrow
        console.log(arrowParent)
 arrowParent.classList.toggle("showMenu");
  });
}

let sidebar = document.querySelector(".sidebar");
let sidebarBtn = document.querySelector(".bx-menu");
console.log(sidebarBtn);
sidebarBtn.addEventListener("click", ()=>{
  sidebar.classList.toggle("close");
});



$(function () {
    $('#ancrResetPassword').click(function () {
        $('#resetPassword_form').submit();
    });
    
});
