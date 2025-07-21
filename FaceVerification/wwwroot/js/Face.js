var ModelData = {
    ImgData: null,
    ImgType: null,
    ImageName: null,
    ImageUrl: null
};

var DefaultImgUrl = "";

jQuery(document).ready(function () {
    "use strict";
    DefaultImgUrl = '/images/NoImage.jpeg';
});
function ConvertFileToBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result.toString().substr(reader.result.toString().indexOf(',') + 1));
        reader.onerror = error => reject(error);
    });
}

async function OpenCamera(ImageId, CameraID) {
    let img = jQuery(`#${CameraID}`);
    jQuery(`#${ImageId}`).attr("src", window.URL.createObjectURL(img[0].files[0]));
    ModelData.ImgData = await ConvertFileToBase64(img[0].files[0]);
    ModelData.ImgType = img[0].files[0].type.split('/')[1];
}
function RemoveImg(ImageId, CameraID) {
    jQuery(`#${CameraID}`).val('');
    ModelData.ImgData = "";
    jQuery(`#${ImageId}`).attr('src', DefaultImgUrl);
}


function ValidateFace() {
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