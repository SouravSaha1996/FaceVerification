var ModelData = {
    UserId: 0,
    ImgData: null,
    ImgType: null,
    ImageName: null,
    ImageUrl: null
};

var DefaultImgUrl = "", video = null, canvas = null, photo = null;

jQuery(document).ready(function () {
    "use strict";
    DefaultImgUrl = '/images/NoImage.jpeg';

    video = document.getElementById('video');
    canvas = document.getElementById('canvas');
    photo = document.getElementById('ImgPriview');
});

// Capture the photo
function TakePhoto() {
    const context = canvas.getContext('2d');
    context.drawImage(video, 0, 0, canvas.width, canvas.height);
    const imageDataURL = canvas.toDataURL('image/png');
    photo.src = imageDataURL;
    ModelData.ImgData = imageDataURL.split(',')[1];
}
function ConvertFileToBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result.toString().substr(reader.result.toString().indexOf(',') + 1));
        reader.onerror = error => reject(error);
    });
}

function OpenCameraWeb() {
    jQuery('#video').removeClass('d-none');
    const constraints = {
        video: true
    };

    // Open the camera
    navigator.mediaDevices.getUserMedia(constraints)
        .then((stream) => {
            video.srcObject = stream;
        })
        .catch((error) => {
            console.error('Camera error:', error);
        });
}

async function OpenCameraMob() {
    let img = jQuery(`#ImgCamera`);
    jQuery(`#ImgPriview`).attr("src", window.URL.createObjectURL(img[0].files[0]));
    ModelData.ImgData = await ConvertFileToBase64(img[0].files[0]);
    ModelData.ImgType = img[0].files[0].type.split('/')[1];
}

function RemoveImg() {
    jQuery('#video').addClass('d-none');
    jQuery(`#ImgCamera`).val('');
    ModelData.ImgData = "";
    jQuery(`#ImgPriview`).attr('src', DefaultImgUrl);
}

function ValidateFace() {
    ModelData.UserId = jQuery('#UserId').val();
    Swal.fire({
        title: 'Loading...',
        html: 'Please wait...',
        allowEscapeKey: false,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading()
        }
    });
    var jsonData = JSON.stringify(ModelData);
    jQuery.ajax({
        type: "Post",
        url: '/Face/ValidateFace',
        data: function () {
            var data = new FormData();
            data.append("modelParameter", jsonData);
            return data;
        }(),
        contentType: false,
        processData: false,
        success: function (data) {
            Swal.close();
            let jsondata = JSON.parse(data);
            if (jsondata.Result == "1") {
                jQuery('#successDiv').removeClass('d-none');
                jQuery('#errorDiv').addClass('d-none');
            } else {
                jQuery('#successDiv').addClass('d-none');
                jQuery('#errorDiv').removeClass('d-none');
                ErrorMessage("Error", jsondata.errorMessage);
            }
        },
        error: function (error) {
            Swal.close();
            console.log(error);
        }
    });
}