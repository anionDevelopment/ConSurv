using ConSurvBackend.Core.Model.Base;
using ConSurvBackend.Core.Model.DTOs;
using ConSurvBackend.Core.Model.RecordModes;
using GRYLibrary.Core.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConSurvBackend.Tests.Testcases.Model
{
    [TestClass]
    public class VideoInformationTests
    {
        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToDTO_MapsAllPublicFields()
        {
            // arrange
            VideoInformation videoInformation = new VideoInformation
            {
                StreamURL = "rtsp://cam.example.com/stream",
                Certificate = "secret-cert",
                SupportsPTZViaONVIF = true,
                ONVIFUrl = "http://cam.example.com/onvif",
                ONVIFUsername = "admin",
                ONVIFPassword = "secret",
            };

            // act
            VideoInformationDTO result = videoInformation.ToDTO();

            // assert
            Assert.AreEqual(videoInformation.StreamURL, result.StreamURL);
            Assert.AreEqual(videoInformation.SupportsPTZViaONVIF, result.SupportsPTZViaONVIF);
            Assert.AreEqual(videoInformation.ONVIFUrl, result.ONVIFUrl);
            Assert.AreEqual(videoInformation.ONVIFUsername, result.ONVIFUsername);
            Assert.AreEqual(videoInformation.ONVIFPassword, result.ONVIFPassword);
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToDTO_DoesNotExposesCertificate()
        {
            // arrange
            VideoInformation videoInformation = new VideoInformation
            {
                StreamURL = "rtsp://cam.example.com/stream",
                Certificate = "this-must-not-leak",
                SupportsPTZViaONVIF = false,
            };

            // act
            VideoInformationDTO result = videoInformation.ToDTO();

            // assert — VideoInformationDTO has no Certificate field by design
            Assert.IsNotNull(result);
            Assert.AreEqual(videoInformation.StreamURL, result.StreamURL);
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void ToDTO_WithNullOptionalFields_PreservesNulls()
        {
            // arrange
            VideoInformation videoInformation = new VideoInformation
            {
                StreamURL = "rtsp://cam.example.com/stream",
                SupportsPTZViaONVIF = false,
                ONVIFUrl = null,
                ONVIFUsername = null,
                ONVIFPassword = null,
            };

            // act
            VideoInformationDTO result = videoInformation.ToDTO();

            // assert
            Assert.IsNull(result.ONVIFUrl);
            Assert.IsNull(result.ONVIFUsername);
            Assert.IsNull(result.ONVIFPassword);
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void VideoInformationDTO_ToVideoInformation_MapsAllFields()
        {
            // arrange
            VideoInformationDTO dto = new VideoInformationDTO
            {
                StreamURL = "rtsp://cam.example.com/stream",
                SupportsPTZViaONVIF = true,
                ONVIFUrl = "http://cam.example.com/onvif",
                ONVIFUsername = "admin",
                ONVIFPassword = "secret",
            };

            // act
            VideoInformation result = dto.ToVideoInformation();

            // assert
            Assert.AreEqual(dto.StreamURL, result.StreamURL);
            Assert.AreEqual(dto.SupportsPTZViaONVIF, result.SupportsPTZViaONVIF);
            Assert.AreEqual(dto.ONVIFUrl, result.ONVIFUrl);
            Assert.AreEqual(dto.ONVIFUsername, result.ONVIFUsername);
            Assert.AreEqual(dto.ONVIFPassword, result.ONVIFPassword);
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void VideoInformationDTO_ToVideoInformation_WithNullOptionalFields_PreservesNulls()
        {
            // arrange
            VideoInformationDTO dto = new VideoInformationDTO
            {
                StreamURL = "rtsp://cam.example.com/stream",
                SupportsPTZViaONVIF = false,
                ONVIFUrl = null,
                ONVIFUsername = null,
                ONVIFPassword = null,
            };

            // act
            VideoInformation result = dto.ToVideoInformation();

            // assert
            Assert.IsNull(result.ONVIFUrl);
            Assert.IsNull(result.ONVIFUsername);
            Assert.IsNull(result.ONVIFPassword);
        }

        [TestMethod]
        [TestProperty(nameof(TestKind), nameof(TestKind.UnitTest))]
        public void UpdateCameraDTO_ToCamera_MapsAllFields()
        {
            // arrange
            string cameraId = "cam-001";
            UpdateCameraDTO dto = new UpdateCameraDTO
            {
                CameraId = cameraId,
                Name = "Entrance cam",
                VideoInformationDTO = new VideoInformationDTO
                {
                    StreamURL = "rtsp://entrance.example.com/stream",
                    SupportsPTZViaONVIF = true,
                    ONVIFUrl = "http://entrance.example.com/onvif",
                    ONVIFUsername = "admin",
                    ONVIFPassword = "secret",
                },
                RecordModeDTO = new RecordModeDTO { RecordMode = nameof(RecordAlways) },
            };

            // act
            Camera result = dto.ToCamera();

            // assert
            Assert.AreEqual(cameraId, result.Id);
            Assert.AreEqual("Entrance cam", result.Name);
            Assert.IsInstanceOfType(result.RecordMode, typeof(RecordAlways));
            Assert.AreEqual(dto.VideoInformationDTO.StreamURL, result.VideoInformation.StreamURL);
            Assert.AreEqual(dto.VideoInformationDTO.SupportsPTZViaONVIF, result.VideoInformation.SupportsPTZViaONVIF);
            Assert.AreEqual(dto.VideoInformationDTO.ONVIFUrl, result.VideoInformation.ONVIFUrl);
        }
    }
}
