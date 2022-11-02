// JavaScript Document

var cropper;

$(document).ready(function(){
	jQuery.fn.exists = function(){return this.length>0;}
	
	if ($('.qrBtn').exists()) {
		$('.qrBtn').click(function(){
			$('.login-hldr').addClass('cam-open');
			$('.try-left').addClass('cam-open');

			// To open the Web Cam
			Webcam.set({
				width: '100%',
				height: '100%',
				image_format: 'jpeg',
				jpeg_quality: 90
			});
			Webcam.attach('#my_camera');
			Webcam.on( 'error', function(err) {
				$('.cameraVideoMsg').show();
			});
		});
	}

	if ($('.qrBtnCls').exists()) {
		$('.qrBtnCls').click(function(){
			$('.login-hldr').removeClass('cam-open');
			$('.try-left').removeClass('cam-open');

			$('#proCameraVideo').hide();
			$('.img-thumb').show();
			$('.open-webcam').show();
			$('.proCameraVideoMsg').hide();
			Webcam.reset('#my_camera');
		});
	}

	if ($('.miriSlider').exists()){
		var miriSlider = $(".miriSlider").owlCarousel({
			items: 1,
			autoheight: true,
			loop: true,
			autoplay: true,
			autoplayTimeout: 10000,
			autoplayHoverPause: true
		});
	}

	if ($('.productLst li').exists()) {
		$('.productLst li').click(function(){
			$('.productLst li').removeClass('active');
			$(this).addClass('active');
			var $thisIndex = $(this).index();
			miriSlider.trigger('to.owl.carousel', $thisIndex);

			var $thisContent = $(this).attr('data-content');
			$('#tryTab').text($thisContent);

			// 23-12-2019
			$('.productBtn').prop('disabled',false);
		});
	}

	if ($('#fileUpload').exists()) {
		$('#fileUpload').change(function(){
			if (this.files && this.files[0]) {
	            var reader = new FileReader();
	            reader.onload = function (e) {
	            	$('.img-thumb').hide();
                    $('.open-webcam').hide();
                   // alert(e.target.result);
                    //if (e.target.result)
	            	$('#image_upload_preview').show();
                    $('#image_upload_preview').attr('src', e.target.result);
                    $('#camimage').attr('value', e.target.result);
	            }
	            reader.readAsDataURL(this.files[0]);
	        }
		});
	}

	if ($('.downQrBtn').exists()) {
		$('.downQrBtn').click(function(){
			$('.downQrBtn').removeClass('active');
			$(this).addClass('active');
			$(this).next().show();
			$('.topTitle').addClass('active');
			$('.main-overlay').show();
		});
	}

	if ($('.main-overlay').exists()){
		$('.main-overlay').click(function(){
			$(this).hide();
			$(this).removeClass('active');
			$('.topTitle').removeClass('active');
			$('.qr-drop').hide();
		});
    }
    /* 02-03-2020 */
    if ($('.qrClose').exists()) {
        $('.qrClose').click(function () {
            $('.main-overlay').trigger('click');
        });
    }
	/* END 02-03-2020 */

	if ($('.noBtn').exists()){
		$('.noBtn').click(function(){
			$('.log-out-sec').removeClass('open');
			$('.ovly').removeClass('open');
		});
	}

	if ($('.logoutBtn').exists()){
		$('.logoutBtn').click(function(){
			$('.log-out-sec').addClass('open');
			$('.ovly').addClass('open');
		});
	}

	if ($('.loginBtn').exists()){
		$('.loginBtn').click(function(){
			if ($('.miriId').val().length > 0){
				$(this).parents('.form-hldr').find('.input-hldr').removeClass('error');
				$(this).parents('.form-hldr').find('.error-txt').removeClass('show');
				/* 07-01-2020 */
				$(this).find('img').css({'opacity':0});
				$(this).find('.loader').show();
				setTimeout(function(){
					//window.location.href = "3.html";
				},2000);
				/* END 07-01-2020 */
			} else{
				$(this).parents('.form-hldr').find('.input-hldr').addClass('error');
				$(this).parents('.form-hldr').find('.error-txt').addClass('show');
				$(this).parents('.form-hldr').find('.input-hldr input').focus();
			}
		});
	}

	if ($('.nextBtn').exists()){
		$('.nextBtn').click(function(){
			var $this = $(this);
			if ($this.hasClass('chkValidation')){
				$('#container .all-fields').addClass('active');
				setTimeout(function(){
					$('#container .all-fields').removeClass('active');

					var $parentIndex = ($this.parents('.main-content').index()) + 1;
					var $wrapperWidth = $('.wrapper').outerWidth();
					$('.main-hldr').css({
						transform: 'translateX(-'+($wrapperWidth * $parentIndex)+'px)'
					});

					$('#container ul.steps li').eq(($parentIndex)+1).addClass('active');
					$('#container ul.steps li').eq(($parentIndex)).addClass('completed');

					// Off the camera
					Webcam.reset('#my_camera');
				},2000);
			} else if ($this.hasClass('showOtp')){
				$('#container .all-fields.failure').addClass('active');

				setTimeout(function(){
					$('#container .all-fields.failure').removeClass('active');
					$('#container .all-fields.success').addClass('active');
				},1500);

				setTimeout(function(){
					$('#container .all-fields.success').removeClass('active');
					$('#container .all-fields.notify').addClass('active');
				},3500);

				setTimeout(function(){
					$('#container .all-fields.notify').removeClass('active');
					$('.otp-popup').show();
					$('.otp-popup-ovly').show();
				},5000);

				// Off the camera
				Webcam.reset('#my_camera');
			} else if ($this.hasClass('showLoader')){
				$(".loader").show();
				setTimeout(function(){
					$(".loader").hide();

					var $parentIndex = ($this.parents('.main-content').index()) + 1;
					var $wrapperWidth = $('.wrapper').outerWidth();
					$('.main-hldr').css({
						transform: 'translateX(-'+($wrapperWidth * $parentIndex)+'px)'
					});

					$('#container ul.steps li').eq(($parentIndex)+1).addClass('active');
                    $('#container ul.steps li').eq(($parentIndex)).addClass('completed');
                    /* 05-02-2020 */
                    if ($('.verPlatformLst').exists()) {
                        $('.verPlatformLst').owlCarousel({
                            /* 15-02-2020 */
                            items: 3,
                            margin: 10,
                            nav: true,
                            navText: ['<span></span>', '<span></span>'],
                            dots: false,
                            /* END 15-02-2020 */
                        });
                    }
					/* END 05-02-2020 */
				},2000);
			} else{
				var $parentIndex = ($this.parents('.main-content').index()) + 1;
				var $wrapperWidth = $('.wrapper').outerWidth();
				$('.main-hldr').css({
					transform: 'translateX(-'+($wrapperWidth * $parentIndex)+'px)'
				});

				$('#container ul.steps li').eq(($parentIndex)+1).addClass('active');
				$('#container ul.steps li').eq(($parentIndex)).addClass('completed');

				// Off the camera
				Webcam.reset('#my_camera');
			}
		});
	}

	if ($('.backBtn').exists()){
		$('.backBtn').click(function(){
			var $parentIndex = ($(this).parents('.main-content').index()) - 1;
			var $wrapperWidth = $('.wrapper').outerWidth();
			$('.main-hldr').css({
				transform: 'translateX(-'+($wrapperWidth * $parentIndex)+'px)'
			});

			$('#container ul.steps li').eq($parentIndex+1).removeClass('active');
			$('#container ul.steps li').eq($parentIndex).removeClass('completed');

			$('.otp-popup').hide();
			$('.otp-popup-ovly').hide();
		});
	}

	if ($('.subBtn').exists()){
		$('.subBtn').click(function(){
            if ($('#subInput').val().length > 0){
				$('.try-left').removeClass('error');
				$('.try-left').addClass('success');
				
				setTimeout(function(){
					$('.try-form').hide();
					$('.try-left').addClass('show');
					$('#subInput').val('');
				},1500);

				setTimeout(function(){
					$('.try-left').removeClass('show');
					$('.try-left').removeClass('success');
					$('.try-left').addClass('active');
					$('.about-sec').addClass('active');
				},2200);
			} else{
				$('.try-form').hide();
				$('.try-left').addClass('error');
			}
		});
	}

	if ($('.notValidRetry').exists()){
		$('.notValidRetry').click(function(){
			$('.try-left').removeClass('error');
			$('.try-form').show();
		});
	}

	if ($('.retryBtn').exists()){
		$('.retryBtn').click(function(){
			$('.about-sec').removeClass('active');
			$('.try-left').removeClass('active');
			$('.try-form').show();
		});
	}

	if ($('.tryMiriId').exists()){
		$('.tryMiriId').click(function(){
			$('.main-hldr').css({
				transform: 'translateX(0)'
			});

			$('#container ul.steps li').removeClass('active').removeClass('completed');
			$('#container ul.steps li').eq(0).addClass('active');
			$('.about-sec').removeClass('active');
			$('.try-left').removeClass('active');
			$('.try-form').show();
		});
	}

	if ($('.webCamBtn').exists()){
		$('.webCamBtn').click(function(){
			$(this).hide();
			$(".img-thumb").hide();
			$('.open-webcam').hide();
			$('#proCameraVideo').show();

			Webcam.set({
				width: '100%',
				height: '100%',
				image_format: 'jpeg',
				jpeg_quality: 90
			});
			Webcam.attach('#proCameraVideo');
			Webcam.on( 'error', function(err) {
				$('#proCameraVideo').hide();
				$('.proCameraVideoMsg').show();
			});
		});
	}

	if ($('.miriInfo').exists()){
		miriInfo();
		$('.miriInfo').mCustomScrollbar({
			theme: "dark",
			scrollInertia: 100
		});
	}

	if ($('.inputClr').exists()){
		$('.inputClr').click(function(){
			$(this).hide();
			$('#miriId').val('');
            $('#miriId').focus();
            
           
		});
	}

	if ($('.miriId').exists()){
		$('.miriId').keyup(function(){
			if ($(this).val().length > 0){
                $('.inputClr').show();
                
			} else{
				$('.inputClr').hide();
			}
		});
	}

	if ($('.cardID').exists()){
		//$('.cardID').on('input propertychange', function() {
		//  var node = $('.cardID')[0]; // vanilla javascript element
		//  var cursor = node.selectionStart; // store cursor position
		//  var lastValue = $('.cardID').val(); // get value before formatting
		  
		//  var formattedValue = formatCardNumber(lastValue);
		//  $('.cardID').val(formattedValue); // set value to formatted
		  
		//  // keep the cursor at the end on addition of spaces
		//  if(cursor === lastValue.length) {
		//    cursor = formattedValue.length;
		//    // decrement cursor when backspacing
		//    // i.e. "4444 |" => backspace => "4444|"
		//    if($('.cardID').attr('data-lastvalue') && $('.cardID').attr('data-lastvalue').charAt(cursor - 1) == " ") {
		//      cursor--;
		//    }
		//  }

		  //if (lastValue !== formattedValue) {
		  //  // increment cursor when inserting character before a space
		  //  // i.e. "1234| 6" => "5" typed => "1234 5|6"
		  //  if(lastValue.charAt(cursor) == " " && formattedValue.charAt(cursor - 1) == " ") {
		  //    cursor++;
		  //  }
		  //}
		  
		  // set cursor position
		//  node.selectionStart = cursor;
		//  node.selectionEnd = cursor;
		//  // store last value
		//  $('.cardID').attr('data-lastvalue', formattedValue);
		//});
	}

	/* MIRI CARD */
	if ($('.productSlider').exists()){
		$(".productSlider").owlCarousel({
			items: 1,
			autoheight: true,
			dots: true
		});
	}

	if ($('.paymentMeth').exists()){
		$('.paymentMeth').change(function(){
			if($(this).is(":checked")){
				$('ul.pay-meth-lst').find('.pay-meth-cnt').slideUp();
				$(this).parents('li').find('.pay-meth-cnt').slideDown();
			}
		}).change();
	}

	if ($('.slideNextBtn').exists()){
		$('.slideNextBtn').click(function(){
			var $this = $(this);
			
			var $slideParentIndex = ($this.parents('.ech-slide').index()) + 1;
			var $slideWrapperWidth = $('.wrapper').outerWidth();
			$('.slide-mn').css({
				transform: 'translateX(-'+($slideWrapperWidth * $slideParentIndex)+'px)'
			});

			$('ul.product-steps li').eq($slideParentIndex).addClass('active');
			$('ul.product-steps li').eq(($slideParentIndex)-1).addClass('completed');
			
			// Off the camera
			Webcam.reset('#my_camera');
		});
	}

	if ($('.cardCam').exists()){
		$('.cardCam').click(function(){
			$(this).hide();
			$('.cam-pic').hide();
			$('.qr-cam').show();

			// To open the Web Cam
			//Webcam.set({
			//	width: '100%',
			//	height: '100%',
			//	image_format: 'jpeg',
			//	jpeg_quality: 90
			//});
			//Webcam.attach('#my_camera');

			/* 30-12-2019 */
			Webcam.on( 'live', function(err) {
				$('.cameraVideoMsg').hide();
				$('#proCameraVideo').show();
			});
			/* END 30-12-2019 */
			
			Webcam.on( 'error', function(err) {
				$('.cameraVideoMsg').show();
			});
		});
	}

	if ($('.flagSelect').exists()){
		$(".flagSelect").CcPicker();
		$(".flagSelect").CcPicker("setCountryByCode","IN");
	}

	if ($('.stepOne').exists()){
		$('.stepOne').click(function () {			
			$(this).hide();
			$('.stepTwo').show();
		});
	}

    if ($('.cardBrand').exists()) {
        
        $('.cardBrand').select2({
           
			placeholder: "Card Brand",
			width: "100%",
			allowClear: false,
			templateResult: format,
			templateSelection: function (option) {
                if (option.id.length > 0)
                {
                    if (option.id == 1)
                    {
                        return "<img src='Content/images/drop-visa.png' alt=''>" + option.text;
                      
                    }
                    else if (option.id == 2)
                    {
                        return "<img src='Content/images/drop-master-card.png' alt=''>" + option.text;
                       
                    }
                    else if (option.id == 3)
                    {
                        return "<img src='Content/images/drop-rupay.png' alt=''>" + option.text;
                      
					}
                }
                else
                {
                   
					return option.text;
				}
			},
			escapeMarkup: function (m) {
				return m;
			}
		});
		
	}

    if ($('.picUpload').exists()) {
  
        $('.picUpload').change(function () {
            var file = document.querySelector("#file");
            if (/\.(jpe?g|png|jpeg)$/i.test(file.files[0].name) === false) {
                swal("Please upload only JPEG & PNG file type.");
            } else
            {
                if (this.files && this.files[0]) {

                
                    var reader = new FileReader();
                    // alert(reader);
                    reader.onload = function (e) {
                        $('.step-one').hide();
                        $('.stepTwo').hide();

                        $('.image_upload_preview_hldr').show();
                        //$('#image_upload_preview').show();
                        //$('#image_upload_preview').attr('src', e.target.result);
                        $('.cropPopup').show();
                        $('.crop-popup-ovly').show();
                        // 21-01-2020
                        //$('#crop-img').attr('src', e.target.result);
                        cropper.replace(e.target.result);
                        // $('#crop-img').load(function(){
                        // 	                	//
                        // 	                }, function(){
                        // 	                	// Once image is loaded
                        // 	                	var image = document.getElementById('crop-img');
                        // 		                cropper = new Cropper(image, {
                        // 					        aspectRatio: 1,
                        // 					        viewMode: 1,
                        // 					        rounded: true
                        // 					    });
                        // 	                });
                        // 21-01-2020
                    }
                    reader.readAsDataURL(this.files[0]);


                }
            }
          
		});
	}

	if ($('.cropBtn').exists()) {
		$('.cropBtn').click(function(){
			var croppedImg = cropper.getCroppedCanvas();
			var finalImg = croppedImg.toDataURL(); // Convert canvas to image
			$('#image_upload_preview').show();
			$('#image_upload_preview').attr('src', finalImg);
            $('#camimage').attr('value', finalImg);
			// Hide webcam
			$('.web-cam-pic').hide();
			$('.image_upload_preview_hldr').show();

			$('.crop-popup-ovly').hide();
            $('.crop-popup').hide();
            // 21-01-2020
            $('.capture-photo-hldr').hide();
            $('.change-photo').show();
		});
    }

    // 12-03-2020
    if ($('.webCamStep').exists()) {
        $('.webCamStep').click(function () {
            $('.web-cam-loader').show();
            $('.step-one').hide();
            $('.stepTwo').hide();
            $('.web-cam-pic').show();

            Webcam.set({
                width: 640,
               // height: 465,
                height: 440,
                image_format: 'jpeg',
                jpeg_quality: 100
            });
            Webcam.attach('#proCameraVideo');
            Webcam.on('live', function (err) {
                $('#proCameraVideo').show();
                $('.capture-photo-hldr').show();

                $('.camPopup').show();
                $('.crop-popup-ovly').show();
                $('.web-cam-loader').hide();
            });
            Webcam.on('error', function (err) {
                $('.web-cam-loader').hide();
                $('#proCameraVideo').hide();
                $('.proCameraVideoMsg').show();
                $('.back-to-photo-hldr').show();
            });
        });
    }

    if ($('.backToPhoto').exists()) {
        $('.backToPhoto').click(function () {
            $(this).parent().hide();
            $('.proCameraVideoMsg').hide();
            $('.web-cam-pic').hide();
            $('.stepOne').css('display', 'flex');
        });
    }
	// END 12-03-2020

 //   if ($('.webCamStep').exists()) {
      
	//	$('.webCamStep').click(function(){
	//		$('.step-one').hide();
	//		$('.stepTwo').hide();
	//		$('.web-cam-pic').show();

	//		Webcam.set({
	//			width: 123,
	//			height: 123,
	//			image_format: 'jpeg',
	//			jpeg_quality: 100
 //           });
        
	//		Webcam.attach('#proCameraVideo');
	//		Webcam.on( 'live', function(err) {
 //               $('#proCameraVideo').show();
 //               $('.capture-photo-hldr').show();
	//		});
	//		Webcam.on( 'error', function(err) {
	//			$('#proCameraVideo').hide();
 //               $('.proCameraVideoMsg').show();
 //               $('.change-photo').show();
	//		});
	//	});
	//}
	/* END MIRI CARD */
    
    /* 21-01-2020 */
    if ($("#crop-img").exists()) {
        var image = document.getElementById('crop-img');
      
        cropper = new Cropper(image, {
            aspectRatio: 1,
            viewMode: 1,
            rounded: true
             
        });
        
    }
    /* 21-01-2020 */
    if ($('.capturePhoto').exists()) {
        $('.capturePhoto').click(function () {
          
            Webcam.snap(function (data_uri) {
                // Off the camera
                Webcam.reset('#my_camera');
                // 12-03-2020
                $('.camPopup').hide();
				// END 12-03-2020
                $('.cropPopup').show();
                $('.crop-popup-ovly').show();
                /* 21-01-2020 */
                //$('#crop-img').attr('src', data_uri);
                cropper.replace(data_uri); // New line added
                /* 21-01-2020 */
                // $('#crop-img').load(function(){
                //                 	//
                //                 }, function(){
                //                 	// Once image is loaded
                //                 	
                //                 });
                /* 21-01-2020 */
            });
        });
    }

    if ($('.changePhoto').exists()) {
        $('.changePhoto').click(function () {
            $(this).parent().hide();
          
            $('.image_upload_preview_hldr').hide();
            $('.image_upload_preview_hldr').find('img').attr('src', "#");
            $('.stepTwo').show();



           $('#stepid').css("position", "absolute");


            $('.proCameraVideoMsg').hide();
            Webcam.reset('#my_camera');
        });
    }
	/* END 21-01-2020 */

	// 23-12-2019
	if ($('.productBtn').exists()){
		$('.productBtn').click(function(){
			var $miriSelected = $('.productLst li.active').attr('data-content');
			if ($miriSelected == "MIRI ID"){
                window.location.href = "/MiriId";
                //window.location.href = "@Url.Action('Index','MiriId')";
			} else if ($miriSelected == "MIRI Token"){
                window.location.href = "/MiriToken";
                //window.location.href = "@Url.Action('Index','MiriToken')";
			}  else if ($miriSelected == "MIRI Card"){
				//window.location.href = "miri-card.html";
                window.location.href = "/MiriCard"
                //window.location.href = "@Url.Action('Index','MiriCard')";
            } else if ($miriSelected == "MIRI Pay") {
                // Miri Pay
            }
            else if ($miriSelected == "MIRI Sso") {
				// Miri Sso
			}
		});
    }

  // 27-03-2020
    if ($('.closeCam').exists()) {
        $('.closeCam').click(function () {
            //alert('hi');
            $('.stepTwo').show();
            $('.web-cam-pic').hide();
            $('.camPopup').hide();
            $('.crop-popup-ovly').hide();
            Webcam.reset('#my_camera');
            
        });
    }

    if ($('.closeCam1').exists()) {
        $('.closeCam1').click(function () {
            $('.stepTwo').show();
            $('.web-cam-pic').hide();
            $('.camPopup').hide();
            $('.crop-popup-ovly').hide();
            $('.crop-popup-ovly').hide();
            $('.crop-popup').hide();
            Webcam.reset('#my_camera');

        });
    }
	// END 27-03-2020

 //   if ($('#proCameraVideo').exists()) {
 //       debugger
 //       $('#proCameraVideo').click(function () {
           
	//		Webcam.snap( function(data_uri) {
	//			// Off the camera
	//			Webcam.reset('#my_camera');

	//			$('.cropPopup').show();
 //               $('.crop-popup-ovly').show();
 //               $('#crop-img').attr('src', data_uri);
 //               $('#crop-img').load(function(){
 //               	//
 //               }, function(){
 //               	// Once image is loaded
 //               	var image = document.getElementById('crop-img');
	//                cropper = new Cropper(image, {
	//			        //
	//			    });
 //               });
	//		});
	//	});
	//}
	// END 23-12-2019
	
});

function format (option) {
	if (!option.id) { return option.text; }
	if (option.id == 1){
		var ob = '<img src="Content/images/drop-visa.png" alt="" />' + option.text;
		return ob;
	} else if (option.id == 2){
        var ob = '<img src="Content/images/drop-master-card.png" alt="" />' + option.text;
		return ob;
	} else{
        var ob = '<img src="Content/images/drop-rupay.png" alt="" />' + option.text;
		return ob;
	}
}

// Window Load
$(window).load(function(){
	$('body').addClass('start');
});

$(window).resize(function(){
	if ($('.main-hldr').exists()){
		var currTrans = $('.main-hldr').css('-webkit-transform').split(/[()]/)[1];
		
		if (currTrans == undefined){
			var posx = 0;
		}else{
			var posx = currTrans.split(',')[4];
		}
		
		$('.main-hldr').css({
			transform: 'translateX(-'+(posx)+'px)'
		});
	}

	if ($('.miriInfo').exists()){
		miriInfo();
	}

	/* 08-01-2020 */
	if ($('.slide-mn').exists()){
		var activeIndex = $('ul.product-steps').find('li.active:last').index();
		var transPos = $('.slide-mn').width();
		
		$('.slide-mn').css({
			transform: 'translateX(-'+(transPos*activeIndex)+'px)'
		});
	}
	/* END 08-01-2020 */
});

function miriInfo(){
	var wrapperHeight = $('.wrapper').outerHeight();
	var headerHeight = $('#header').height();
	var stepsHeight = $('.steps').outerHeight(true);
	var ttlHeight = ((wrapperHeight) - (headerHeight+stepsHeight)) - 57;

	$('.miriInfo').css({
		'height': ttlHeight
	});
}

function formatCardNumber(value) {
  // remove all non digit characters
  var value = value.replace(/\D/g, '');
  var formattedValue;
  var maxLength;
  // american express, 15 digits
  if ((/^3[47]\d{0,13}$/).test(value)) {
    formattedValue = value.replace(/(\d{4})/, '$1 ').replace(/(\d{4}) (\d{6})/, '$1 $2 ');
    maxLength = 17;
  } else if((/^3(?:0[0-5]|[68]\d)\d{0,11}$/).test(value)) { // diner's club, 14 digits
    formattedValue = value.replace(/(\d{4})/, '$1 ').replace(/(\d{4}) (\d{6})/, '$1 $2 ');
    maxLength = 16;
  } else if ((/^\d{0,16}$/).test(value)) { // regular cc number, 16 digits
    formattedValue = value.replace(/(\d{4})/, '$1 ').replace(/(\d{4}) (\d{4})/, '$1 $2 ').replace(/(\d{4}) (\d{4}) (\d{4})/, '$1 $2 $3 ');
    maxLength = 19;
  }
  
  $('.cardID').attr('maxlength', maxLength);
  return formattedValue;
}


/* Page Loader 27-12-2019 */
$(window).load(function(){
	setTimeout(function(){
		$('.home-loader').hide();
	},2000);
});
/* END Page Loader 27-12-2019 */


