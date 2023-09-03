$(document).ready(function(){

    
	$('#player').videre({
		video: {
			quality: [
				{
					label: '720p',
					src: $("#videofile").val()
				},
				{
					label: '360p',
					src: $("#videofile").val()
				},
				{
					label: '240p',
					src: $("#videofile").val()
				}
			],
			title: ''
		},
		dimensions: 1280
	});
	closeProgressIndicator();
});