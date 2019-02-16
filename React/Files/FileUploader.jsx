import React from "react";
import ReactCrop from "react-image-crop";
import "../../styles/reactCrop.css";
import * as fileService from "../../services/filesService";
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from "reactstrap";
import * as notificationsMessage from "../NotificationMessage";

class FileUploader extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      selectedFileName: null,
      src: null,
      modalIsOpen: false,
      crop: {
        aspect: props.aspect === undefined ? 1.8 : props.aspect,
        width: 50,
        x: 0,
        y: 0
      },
      buttonEditHide: "d-none",
      buttonApplyHide: "d-none",
      inputHide: "d-none",
      fileUploader: "",
      cropHide: "",
      alertMesseges: props.alertMesseges === false ? false : true,
      settings: {
        modal: props.modal === false ? false : true
      }
    };
    this.onSelectFile = this.onSelectFile.bind(this);
    this.submitHeandler = this.submitHeandler.bind(this);
  }

  onSelectFile = e => {
    if (e.target.files && e.target.files.length > 0) {
      const reader = new FileReader();
      reader.addEventListener("load", () =>
        this.setState({
          src: reader.result
        })
      );
      if (this.state.settings.modal === true) {
        this.toggle();
        this.setState({
          buttonEditHide: ""
        });
      }
      this.setState({
        selectedFileName: e.target.files[0].name
      });
      reader.readAsDataURL(e.target.files[0]);
    }
  };

  onImageLoaded = (image, pixelCrop) => {
    this.imageRef = image;
  };

  onCropComplete = (crop, pixelCrop) => {
    this.makeClientCrop(crop, pixelCrop);
  };

  onCropChange = crop => {
    this.setState({ crop });
  };
  submitHeandler = () => {
    this.toggle();

    if (this.state.selectedFileName !== null) {
      var Pic = document.getElementById("myCanvas").toDataURL("image/jpg");
      var arr = Pic.split(","),
        mime = arr[0].match(/:(.*?);/)[1],
        bstr = atob(arr[1]),
        n = bstr.length,
        u8arr = new Uint8Array(n);
      while (n--) {
        u8arr[n] = bstr.charCodeAt(n);
      }

      var file = new File([u8arr], this.state.selectedFileName, {
        type: mime
      });
      var requestFile = new FormData();
      requestFile.append("image", file, this.state.selectedFileName);

      fileService
        .uplodeFile(requestFile)
        .then(response => {
          if (this.state.alertMesseges === true) {
            notificationsMessage.success({ message: "Saved Succsessfull!" });
          }
          if (this.props.responseHendler !== undefined) {
            this.props.responseHendler(response);
          }
          this.setState({
            buttonEditHide: "d-none",
            buttonApplyHide: "d-none",
            inputHide: "",
            fileUploader: "d-none"
          });
          if (this.state.settings.modal === false) {
            this.setState({
              cropHide: "d-none"
            });
          }
        })
        .catch(response => {
          if (this.state.alertMesseges === true) {
            notificationsMessage.error({ message: "Faild to Save!" });
          }
          if (this.props.responseHendler !== undefined) {
            this.props.responseHendler(response);
          }
        });
    }
  };
  async makeClientCrop(crop, pixelCrop) {
    if (this.imageRef && crop.width && crop.height) {
      var croppedImageUrl = await this.getCroppedImg(
        this.imageRef,
        pixelCrop,
        "newFile.jpeg"
      );
      this.setState({ croppedImageUrl });
    }
  }
  toggle = () => {
    this.setState({
      modalIsOpen: !this.state.modalIsOpen
    });
  };

  getCroppedImg(image, pixelCrop, fileName) {
    this.setState({
      buttonApplyHide: ""
    });
    const canvas = document.getElementById("myCanvas");
    canvas.width = pixelCrop.width;
    canvas.height = pixelCrop.height;
    const ctx = canvas.getContext("2d");
    ctx.drawImage(
      image,
      pixelCrop.x,
      pixelCrop.y,
      pixelCrop.width,
      pixelCrop.height,
      0,
      0,
      pixelCrop.width,
      pixelCrop.height
    );

    return new Promise(resolve => {
      canvas.toBlob(blob => {
        blob.name = fileName;
        window.URL.revokeObjectURL(this.fileUrl);
        this.fileUrl = window.URL.createObjectURL(blob);
        resolve(this.fileUrl);
      }, "image/jpg");
    });
  }
  render() {
    const { crop, croppedImageUrl, src } = this.state;
    let pictureInput = {
      marginTop: "5px",
      backgroundColor: "#FFFFFF",
      border: "1px solid #999",
      borderRadius: "3px",
      padding: "6px 6px"
    };
    let pictureResult = {
      marginTop: "5px",
      backgroundColor: "#FFFFFF",
      border: "1px solid #999",
      borderRadius: "3px",
      padding: "2px 2px"
    };
    let mainDiv = { margin: "5px" };
    let button = {
      padding: "7px 10px",
      borderRadius: "4px",
      backgroundColor: "#23b7e5",
      border: "none",
      color: "#FFFFFF",
      textAlign: "center",
      WebkitTransitionDuration: "0.4s",
      transitionDuration: "0.4s",
      margin: "5px",
      textDecoration: " none",
      fontSize: "16px",
      cursor: " pointer"
    };
    let inputFile = {
      marginRight: "5px",
      content: "'Select some files'",
      display: "inline-block",
      background: "linear-gradient(top, #f9f9f9, #e3e3e3)",
      border: "1px solid #999",
      borderRadius: "3px",
      padding: "5px 8px",
      outline: "none",
      whiteSpace: "nowrap",
      WebkitUserSelect: "none",
      cursor: "pointer",
      textShadow: "1px 1px #fff",
      fontWeight: "700",
      fontSize: "10pt"
    };
    let inputResult = {
      marginRight: "5px",
      content: "'Select some files'",
      display: "inline-block",
      background: "linear-gradient(top, #f9f9f9, #e3e3e3)",
      border: "1px solid #999",
      borderRadius: "3px",
      padding: "8px 10px",
      outline: "none",
      whiteSpace: "nowrap",
      WebkitUserSelect: "none",
      cursor: "pointer",
      textShadow: "1px 1px #fff",
      fontWeight: "700",
      fontSize: "10pt"
    };

    return (
      //alertMesseges
      <div style={mainDiv}>
        <canvas id="myCanvas" className="d-none" />
        <div>
          <span>
            <input
              disabled
              style={
                this.props.inputResultStyle !== undefined
                  ? this.props.inputResultStyle
                  : this.props.inputResultClass !== undefined
                  ? {}
                  : inputResult
              }
              placeholder={`Uploaded: ${this.state.selectedFileName}`}
              className={`${this.state.inputHide} ${
                this.props.inputResultClass
              }`}
            />
            <input
              type="file"
              style={
                this.props.inputFileStyle !== undefined
                  ? this.props.inputFileStyle
                  : this.props.inputFileClass !== undefined
                  ? {}
                  : inputFile
              }
              onChange={this.onSelectFile}
              className={`${this.state.fileUploader} ${
                this.props.inputFileClass
              }`}
            />
            {this.state.settings.modal === true ? (
              <button
                type="button"
                style={
                  this.props.buttonStyle !== undefined
                    ? this.props.buttonStyle
                    : this.props.buttonClass !== undefined
                    ? {}
                    : button
                }
                className={`${this.state.buttonEditHide} ${
                  this.props.buttonClass
                }`}
                onClick={this.toggle}
              >
                Finish Uploading
              </button>
            ) : (
              <button
                type="button"
                style={
                  this.props.buttonStyle !== undefined
                    ? this.props.buttonStyle
                    : this.props.buttonClass !== undefined
                    ? {}
                    : button
                }
                className={`${this.state.buttonApplyHide} ${
                  this.props.buttonClass
                }`}
                onClick={this.submitHeandler}
              >
                Upload
              </button>
            )}
          </span>
        </div>
        {this.state.settings.modal === false ? (
          <div className={this.state.cropHide}>
            <div>
              {src && (
                <ReactCrop
                  style={pictureInput}
                  src={src}
                  crop={crop}
                  onImageLoaded={this.onImageLoaded}
                  onComplete={this.onCropComplete}
                  onChange={this.onCropChange}
                />
              )}
            </div>
            <div>
              {croppedImageUrl && (
                <img
                  style={pictureResult}
                  alt="Crop"
                  width={`${this.props.aspect * 150}`}
                  height="150"
                  src={croppedImageUrl}
                />
              )}
            </div>
          </div>
        ) : (
          <div>
            <Modal
              isOpen={this.state.modalIsOpen}
              toggle={this.toggle}
              className={this.props.className}
            >
              <ModalHeader toggle={this.toggle}>Crop the Image</ModalHeader>
              <ModalBody>
                <div>
                  <div>
                    {src && (
                      <ReactCrop
                        style={pictureInput}
                        src={src}
                        crop={crop}
                        onImageLoaded={this.onImageLoaded}
                        onComplete={this.onCropComplete}
                        onChange={this.onCropChange}
                      />
                    )}
                  </div>
                  <div>
                    {croppedImageUrl && (
                      <img
                        style={pictureResult}
                        alt="Crop"
                        width={
                          this.props.aspect === 0
                            ? "150"
                            : `${this.props.aspect * 150}`
                        }
                        height="150"
                        src={croppedImageUrl}
                      />
                    )}
                  </div>
                </div>
              </ModalBody>
              <ModalFooter>
                <button
                  type="button"
                  style={
                    this.props.buttonStyle !== undefined
                      ? this.props.buttonStyle
                      : this.props.buttonClass !== undefined
                      ? {}
                      : button
                  }
                  className={`${this.state.buttonApplyHide} ${
                    this.props.buttonClass
                  }`}
                  onClick={this.submitHeandler}
                >
                  Upload
                </button>
              </ModalFooter>
            </Modal>
          </div>
        )}
      </div>
    );
  }
}

export default FileUploader;
