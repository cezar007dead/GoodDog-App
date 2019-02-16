import React from "react";
import { Form, Formik } from "formik";
import ContentWrapper from "../Layout/ContentWrapper";
import {
  Row,
  Button,
  Label,
  Input,
  Modal,
  ModalHeader,
  ModalBody,
  ModalFooter
} from "reactstrap";
import * as sponsorServices from "../../services/sponsorServices";
import * as schemas from "../../models/sponsorSchemas";
import * as notificationsMessage from "../NotificationMessage";
import AddressCreation from "../Addresses/AddressCreation";
import AddressUpdate from "../Addresses/AddressUpdate";
import * as addressService from "../../services/addressService";

class AddSponsor extends React.Component {
  constructor(props) {
    super(props);
    this.validation = schemas.getSponsorSchema;
    this.state = {
      submitButtomText: "Add Sponsor",
      sponsorTypes: [],
      primarySponsorTypeId: 1,
      types: [],
      modalIsOpen: false,
      addressId: undefined,
      AddAddressButtom: "",
      addressValue: null
    };
    this.state.sponsor = this.validation.initialValues;
    this.onUpdateSponsorSucsses = this.onUpdateSponsorSucsses.bind(this);
  }
  componentDidMount() {
    if (this.props.match.params.uid !== undefined) {
      sponsorServices
        .getById(this.props.match.params.uid)
        .then(this.onGetSponsorSucsses)
        .catch(this.onGetSponsorError);
    }
    sponsorServices
      .getTypes()
      .then(this.onGetTypesSucsses)
      .catch(this.onGetTypesError);
  }
  handleSubmitSponsor = (values, obj) => {
    console.log(this.state.lol);
    var data = {
      name: values.name,
      companyUrl: values.companyUrl,
      addressId: this.state.addressId,
      phoneNumber: values.phoneNumber,
      contactPerson: values.contactPerson,
      primarySponsorTypeId: values.sponsorType
    };
    if (this.props.match.params.uid === undefined) {
      sponsorServices
        .add(data)
        .then(this.onAddSponsorSucsses)
        .catch(this.onAddSponsorError);
    } else {
      sponsorServices
        .update(data, this.props.match.params.uid)
        .then(this.onUpdateSponsorSucsses)
        .catch(this.onUpdateSponsorError);
    }
  };

  onGetSponsorSucsses = response => {
    addressService
      .getById(response.item.address.id)
      .then(r => {
        this.setState({
          addressValue: `${r.item.lineOne}, ${r.item.city}, ${
            r.item.stateProvince.code
          }`,
          addressId: r.item.id
        });
      })
      .catch(() => {
        console.log("Error on Address getById (AddSponsor.jsx)");
      });

    this.setState({
      sponsor: {
        name: response.item.name,
        companyUrl: response.item.companyUrl,
        address: response.item.address.id,
        phoneNumber: response.item.phoneNumber,
        contactPerson: response.item.contactPerson,
        primarySponsorTypeId: response.item.sponsorType.id,
        sponsorType: response.item.sponsorType.id
      },
      submitButtomText: "Update Sponsor"
    });
  };
  onGetSponsorError = response => {
    console.log("ERROR!");
  };

  onAddSponsorSucsses = response => {
    notificationsMessage.success({ message: "Created Succsessfull!" });
    this.props.history.push({
      pathname: "/sponsors/display"
    });
  };

  onAddSponsorError = response => {
    notificationsMessage.error({ message: "Faild to Create!" });
    console.log("ERROR!");
  };
  onUpdateSponsorSucsses = response => {
    notificationsMessage.success({ message: "Updated Successfull!" });
    if (this.props.history.location.backLocation !== undefined) {
      this.props.history.push({
        pathname: this.props.history.location.backLocation,
        state: { icon: this.props.history.location.icon } // sending back icon beacuse now its another props
      });
    } else {
      this.props.history.push({
        pathname: "/sponsors/display"
      });
    }
  };

  onUpdateSponsorError = response => {
    notificationsMessage.error({ message: "Updated Failed!" });
    console.log("ERROR!");
  };
  onGetTypesSucsses = response => {
    this.setState({
      sponsorTypes: response.items
    });
  };
  onGetTypesError = response => {
    console.log(response);
  };
  toggle = () => {
    this.setState({
      modalIsOpen: !this.state.modalIsOpen
    });
  };
  UpdateAddressResponse = response => {
    if (response.isSuccessful === true) {
      this.toggle();
    }
    addressService
      .getById(this.state.addressId)
      .then(r => {
        this.setState({
          addressValue: `${r.item.lineOne}, ${r.item.city}, ${
            r.item.stateProvince.code
          }`,
          addressId: r.item.id
        });
      })
      .catch(() => {
        console.log("Error on Address getById (AddSponsor.jsx)");
      });
  };
  AddAddressResponse = response => {
    if (response.isSuccessful === true) {
      this.toggle();
    }
    addressService
      .getById(response.item)
      .then(r => {
        this.setState({
          addressValue: `${r.item.lineOne}, ${r.item.city}, ${
            r.item.stateProvince.code
          }`,
          addressId: r.item.id,
          AddAddressButtom: "d-none"
        });
      })
      .catch(() => {
        console.log("Error on Address getById (AddSponsor.jsx)");
      });
  };
  renderOptions = type => {
    return (
      <option key={type.id} value={type.id}>
        {type.name}
      </option>
    );
  };

  render() {
    const disabledIput = { marginTop: "13px" };
    return (
      <ContentWrapper>
        <Formik
          enableReinitialize={true}
          initialValues={this.state.sponsor}
          onSubmit={this.handleSubmitSponsor}
          validationSchema={this.validation()}
        >
          {props => {
            const {
              values,
              touched,
              errors,
              handleChange,
              handleBlur,
              handleSubmit
            } = props;
            return (
              <Row>
                <div className="col-lg-12">
                  <Form
                    className="form-horizontal"
                    action="#"
                    data-parsley-validate=""
                    noValidate=""
                  >
                    <div className="col-md-6">
                      <div className="card card-default">
                        <div className="card-header">
                          <div className="card-title">
                            {this.state.submitButtomText}
                          </div>
                        </div>

                        <div className="card-body">
                          <fieldset>
                            <div className="Form-group Row">
                              <Label className=" col-form-label">Name</Label>
                              <div>
                                <Input
                                  type="text"
                                  id="name"
                                  placeholder="Enter the Name of a Company"
                                  value={values.name}
                                  onChange={handleChange}
                                  onBlur={handleBlur}
                                  className={
                                    errors.name && touched.name ? "error" : ""
                                  }
                                />
                              </div>
                            </div>
                          </fieldset>
                          <fieldset>
                            <div className="Form-group Row">
                              <Label className=" col-form-label">
                                Company Website
                              </Label>
                              <div>
                                <Input
                                  type="text"
                                  id="companyUrl"
                                  placeholder="Enter the URL to Website"
                                  value={values.companyUrl}
                                  onChange={handleChange}
                                  onBlur={handleBlur}
                                  className={
                                    errors.companyUrl && touched.companyUrl
                                      ? "error"
                                      : ""
                                  }
                                />
                                {errors.companyUrl && touched.companyUrl && (
                                  <label className="error">
                                    {errors.companyUrl}
                                  </label>
                                )}
                              </div>
                            </div>
                          </fieldset>
                          <fieldset>
                            <div className="Form-group Row">
                              {this.props.match.params.uid !== undefined &&
                              this.state.sponsor.address !== 0 ? (
                                <div>
                                  <button
                                    type="button"
                                    className={`btn btn-sm btn-outline-primary ${
                                      this.state.AddAddressButtom
                                    }`}
                                    onClick={this.toggle}
                                  >
                                    Edit Address
                                  </button>
                                  <div>
                                    <Modal
                                      isOpen={this.state.modalIsOpen}
                                      toggle={this.toggle}
                                      className={this.props.className}
                                    >
                                      <ModalHeader toggle={this.toggle} />
                                      <ModalBody>
                                        <div>
                                          <AddressUpdate
                                            responseHandler={
                                              this.UpdateAddressResponse
                                            }
                                            addressId={this.state.addressId}
                                            {...this.props.history}
                                          />
                                        </div>
                                      </ModalBody>
                                      <ModalFooter />
                                    </Modal>
                                  </div>
                                  <div style={disabledIput}>
                                    <Input
                                      multiple
                                      disabled
                                      type="text"
                                      id="address"
                                      placeholder="No Address"
                                      value={this.state.addressValue}
                                    />
                                  </div>
                                </div>
                              ) : (
                                <div>
                                  <button
                                    type="button"
                                    className={`btn btn-sm btn-outline-primary ${
                                      this.state.AddAddressButtom
                                    }`}
                                    onClick={this.toggle}
                                  >
                                    Add Address
                                  </button>
                                  <div>
                                    <Modal
                                      isOpen={this.state.modalIsOpen}
                                      toggle={this.toggle}
                                      className={this.props.className}
                                    >
                                      <ModalHeader toggle={this.toggle} />
                                      <ModalBody>
                                        <div>
                                          <AddressCreation
                                            responseHandler={
                                              this.AddAddressResponse
                                            }
                                            {...this.props.history}
                                          />
                                        </div>
                                      </ModalBody>
                                      <ModalFooter />
                                    </Modal>
                                  </div>
                                  <div style={disabledIput}>
                                    <Input
                                      multiple
                                      disabled
                                      type="text"
                                      id="address"
                                      placeholder="No Address"
                                      value={this.state.addressValue}
                                    />
                                  </div>
                                </div>
                              )}
                            </div>
                          </fieldset>
                          <fieldset>
                            <div className="Form-group Row">
                              <Label className=" col-form-label">
                                Phone Number
                              </Label>
                              <div>
                                <Input
                                  type="text"
                                  id="phoneNumber"
                                  placeholder="Enter the Phone Number"
                                  value={values.phoneNumber}
                                  onChange={handleChange}
                                  onBlur={handleBlur}
                                  className={
                                    errors.phoneNumber && touched.phoneNumber
                                      ? "error"
                                      : "bfh-phone"
                                  }
                                />
                                {errors.phoneNumber && touched.phoneNumber && (
                                  <label className="error">
                                    {errors.phoneNumber}
                                  </label>
                                )}
                              </div>
                            </div>
                          </fieldset>
                          <fieldset>
                            <div className="Form-group Row">
                              <Label className=" col-form-label">
                                Contact Person
                              </Label>
                              <div>
                                <Input
                                  type="text"
                                  id="contactPerson"
                                  placeholder="Enter the Name of Person"
                                  value={values.contactPerson}
                                  onChange={handleChange}
                                  onBlur={handleBlur}
                                  className={
                                    errors.contactPerson &&
                                    touched.contactPerson
                                      ? "error"
                                      : ""
                                  }
                                />
                                {errors.contactPerson &&
                                  touched.contactPerson && (
                                    <label className="error">
                                      {errors.contactPerson}
                                    </label>
                                  )}
                              </div>
                            </div>
                          </fieldset>
                          <fieldset>
                            <Label className=" col-form-label">
                              Sponsor Type
                            </Label>
                            <select
                              className="form-control"
                              name="sponsorType"
                              value={values.sponsorType}
                              onChange={handleChange}
                              onBlur={handleBlur}
                              style={{ display: "block" }}
                            >
                              <option value="" label="Select Type" />
                              {this.state.sponsorTypes.map(this.renderOptions)}
                            </select>
                            {errors.sponsorType && touched.sponsorType && (
                              <div className="input-feedback">
                                {errors.sponsorType}
                              </div>
                            )}
                          </fieldset>
                        </div>
                        <div className="card-footer text-center">
                          <Button
                            color="primary"
                            type="button"
                            onClick={handleSubmit}
                            className="submitForm"
                          >
                            {this.state.submitButtomText}
                          </Button>
                        </div>
                      </div>
                    </div>
                  </Form>
                </div>
              </Row>
            );
          }}
        </Formik>
      </ContentWrapper>
    );
  }
}

// <FileUploader
//     aspect={16/9} // or 0
//     alertMesseges={false} // turns off alert messeges
//     responseHendler={"YOUR FUNCTION"} // Sends a response of upload
//     inputFileStyle={{"YOUR STYLE"}} // File Input  Type(Input)
//     inputFileClass={"YOUR CLASSES"}  // File Input Type(Input)
//     inputResultStyle ={{"YOUR STYLE"}} //Result TextBox Type(Input)
//     inputResultClass = {"YOUR CLASSES"} //Result TextBox Type(Input)
//     buttonStyle = {{"YOUR STYLE"}} // Buttons
//     buttonClass = {"YOUR CLASSES"} // Buttons
// ></FileUploader>
export default AddSponsor;
