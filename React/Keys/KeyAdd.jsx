import React from "react";
import { Form, Formik } from "formik";
import ContentWrapper from "../Layout/ContentWrapper";
import { Row, Button, Label, Input, FormGroup } from "reactstrap";
import * as keysServices from "../../services/keysService";
import * as schemas from "../../models/keySchemas";
import * as notificationsMessage from "../NotificationMessage";

class KeyAdd extends React.Component {
  constructor(props) {
    super(props);
    this.validation = schemas.getKeysSchema;
    this.state = {
      submitButtomText: "Add Key",
      checked: false,
      dataTypes: [],
      keys: []
    };
    this.state.key = this.validation.initialValues;
    this.handleChange = this.handleChange.bind(this);
  }
  componentDidMount() {
    if (this.props.match.url === "/key/edit") {
      this.setState({ submitButtomText: "Edit Key" });
      keysServices
        .get()
        .then(this.onGetKeysSucsses)
        .catch(this.onGetKeysError);
    }
    keysServices
      .getDataTypes()
      .then(this.onGetDataTypesSucsses)
      .catch(this.onGetDataTypesError);
  }

  onGetKeysSucsses = response => {
    this.setState({ keys: response.items });
  };
  onGetKeysError = () => {};
  onGetDataTypesSucsses = response => {
    this.setState({ dataTypes: response.items });
  };
  onGetDataTypesError = () => {};

  handleSubmit = (values, obj) => {
    var data = {
      KeyName: values.name,
      Value: values.keyValue,
      DataTypeId: values.dataType,
      IsSecured: values.isSequred == "" ? false : values.isSequred
    };
    if (this.props.match.url === "/key/edit") {
      keysServices
        .update(data)
        .then(this.onUpdateKeySucsses)
        .catch(this.onUpdateKeyError);
    } else {
      keysServices
        .add(data)
        .then(this.onAddKeySucsses)
        .catch(this.onAddKeyError);
    }
  };
  onUpdateKeySucsses = () => {
    notificationsMessage.success({ message: "Updated Succsessfull" });
  };
  onUpdateKeyError = () => {
    notificationsMessage.error({ message: "Faild to Update!" });
  };
  handleChange = e => {
    if (e.target.selectedOptions[0].value === "") {
      this.setState({
        key: {
          name: "",
          keyValue: "",
          dataType: "",
          isSequred: ""
        }
      });
    } else {
      var key = this.state.keys.find(function(element) {
        return element.id === parseInt(e.target.selectedOptions[0].id);
      });
      this.setState({
        key: {
          name: key.keyName,
          keyValue: key.value,
          dataType: key.dataType.id,
          isSequred: key.isSecured
        }
      });
    }
  };
  onAddKeySucsses = () => {
    notificationsMessage.success({ message: "Created Succsessfull!" });
  };
  onAddKeyError = () => {
    notificationsMessage.error({ message: "Faild to Create!" });
  };
  renderOptionsKeys = key => {
    return (
      <option key={key.id} id={key.id} value={key.keyName}>
        {key.keyName}
      </option>
    );
  };
  renderOptionsTypes = type => {
    return (
      <option key={type.id} value={type.id}>
        {type.displayName}
      </option>
    );
  };
  render() {
    return (
      <ContentWrapper>
        <Formik
          enableReinitialize={true}
          initialValues={this.state.key}
          onSubmit={this.handleSubmit}
          validationSchema={this.validation()}
          onChange={this.handleChange}
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
                    onChange={handleChange}
                    onSubmit={handleSubmit}
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
                          {this.props.match.url === "/key/edit" ? (
                            <fieldset>
                              <Label className=" col-form-label">
                                Select the type of Key Value
                              </Label>
                              <select
                                className="form-control"
                                name="name"
                                value={values.name}
                                onChange={this.handleChange}
                                onBlur={handleBlur}
                                style={{ display: "block" }}
                              >
                                <option value="" label="Select Key" />
                                {this.state.keys.map(this.renderOptionsKeys)}
                              </select>
                              {errors.name && touched.name && (
                                <div className="input-feedback">
                                  {errors.name}
                                </div>
                              )}
                            </fieldset>
                          ) : (
                            <fieldset>
                              <div className="Form-group Row">
                                <Label className=" col-form-label">
                                  Name of the Key
                                </Label>
                                <div>
                                  <Input
                                    type="text"
                                    id="name"
                                    placeholder="Enter the Name of a Key"
                                    value={values.name}
                                    onChange={handleChange}
                                    onBlur={handleBlur}
                                    className={
                                      errors.name && touched.name ? "error" : ""
                                    }
                                  />
                                  {errors.name && touched.name && (
                                    <label className="error">
                                      {errors.name}
                                    </label>
                                  )}
                                </div>
                              </div>
                            </fieldset>
                          )}
                          <fieldset>
                            <div className="Form-group Row">
                              <Label className=" col-form-label">
                                Value of the Key
                              </Label>
                              <div>
                                <Input
                                  type="text"
                                  id="keyValue"
                                  placeholder="Enter the Value of the Key"
                                  value={values.keyValue}
                                  onChange={handleChange}
                                  onBlur={handleBlur}
                                  className={
                                    errors.keyValue && touched.keyValue
                                      ? "error"
                                      : ""
                                  }
                                />
                                {errors.keyValue && touched.keyValue && (
                                  <label className="error">
                                    {errors.keyValue}
                                  </label>
                                )}
                              </div>
                            </div>
                          </fieldset>{" "}
                          <fieldset>
                            <Label className=" col-form-label">
                              Select the type of Key Value
                            </Label>
                            <select
                              className="form-control"
                              name="dataType"
                              value={values.dataType}
                              onChange={handleChange}
                              onBlur={handleBlur}
                              style={{ display: "block" }}
                            >
                              <option value="" label="Select Type" />
                              {this.state.dataTypes.map(
                                this.renderOptionsTypes
                              )}
                            </select>
                            {errors.dataType && touched.dataType && (
                              <div className="input-feedback">
                                {errors.dataType}
                              </div>
                            )}
                          </fieldset>
                          <fieldset>
                            <div>
                              <FormGroup check>
                                <Label check>
                                  <Input
                                    type="checkbox"
                                    name="isSequred"
                                    checked={values.isSequred}
                                    value={values.isSequred}
                                    onChange={handleChange}
                                    onBlur={handleBlur}
                                  />
                                  Make it sequred
                                </Label>
                              </FormGroup>
                            </div>
                          </fieldset>
                        </div>
                        <div className="card-footer text-center">
                          <Button
                            color="primary"
                            type="submit"
                            onClick={props.onSubmit}
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

export default KeyAdd;
