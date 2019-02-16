import React from "react";
import * as sponsorServices from "../../services/sponsorServices";
import { CardFooter } from "reactstrap";
import Moment from "react-moment";
import moment from "moment";
import * as notificationsMessage from "../NotificationMessage";
import icons from "./SponsorsConstatns";

class CardSponsor extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      sponsor: {
        id: this.props.data.id,
        name: this.props.data.name,
        companyUrl: this.props.data.companyUrl,
        address: this.props.data.address.id,
        phoneNumber: this.props.data.phoneNumber,
        contactPerson: this.props.data.contactPerson,
        userId: this.props.data.user.id,
        dateCreated: this.props.data.dateCreated
      },
      addressId: this.props.data.address.id
    };

    Moment.globalMoment = moment;
  }
  handleEdit = () => {
    this.props.history.push({
      pathname: "/sponsors/" + this.state.sponsor.id + "/edit",
      backLocation: this.props.history.location.pathname
    });
  };
  handleDelete = () => {
    notificationsMessage.prompt(
      {
        title: "Are you sure that you want to delete Sponsor?",
        text: "Once deleted, you will not be able to recover this!",
        icon: "warning",
        buttons: true,
        dangerMode: true
      },
      this.delete
    );
  };
  delete = willDelete => {
    if (willDelete) {
      sponsorServices
        .remove(this.state.sponsor.id)
        .then(this.onDeleteSponsorSucsses)
        .catch(this.onDeleteSponsorError);
    }
  };
  componentDidMount() {
    if (this.props.data.address.id === 0) {
      let newSponsor = { ...this.state.sponsor };
      newSponsor.address = `No Address`;

      this.setState({
        sponsor: newSponsor
      });
    } else {
      let newSponsor = { ...this.state.sponsor };
      newSponsor.address = `${this.props.data.address.lineOne}, ${
        this.props.data.address.city
      }, ${this.props.data.address.stateProvince.code}`;

      this.setState({
        sponsor: newSponsor
      });
    }
  }

  onDeleteSponsorSucsses = response => {
    notificationsMessage.success({ message: "Deleted Successfull!" });
    window.location.reload();
  };
  onDeleteSponsorError = response => {
    notificationsMessage.success({ message: "Delete Failed!" });
    console.log("ERROR!");
  };
  formatPhoneNumber = phoneNumberString => {
    var cleaned = ("" + phoneNumberString).replace(/\D/g, "");
    var match = cleaned.match(/^(\d{3})(\d{3})(\d{4})$/);
    if (match) {
      return "(" + match[1] + ") " + match[2] + "-" + match[3];
    }
    return null;
  };

  render() {
    let toolTip = moment(this.state.sponsor.dateCreated).format("LLL");
    let phone = {
      Fontsize: "15px"
    };
    return (
      <div className="col-lg-4 col-md-6">
        <div className="card card-default">
          <div className="card-body">
            <div className="row">
              <div className="col">
                <span>
                  <small className="mr-1">
                    By
                    <a className="ml-1 text-primary">
                      {this.props.data.user.userName}
                    </a>
                  </small>
                  <small className="mr-1">
                    <Moment
                      data-toggle="tooltip"
                      data-placement="top"
                      title={toolTip}
                      format="MMMM Do YYYY"
                      date={this.state.sponsor.dateCreated}
                    />
                  </small>
                </span>
              </div>
              <div className="col-2">
                <em
                  className={
                    "fa-2x mb-3 " + icons.ic[this.props.data.sponsorType.id]
                  }
                />
              </div>
            </div>
            <span className="ml-auto" />

            <div className="row">
              <div className="col">
                <h4>
                  <a className="text-primary">{this.state.sponsor.name}</a>
                </h4>
              </div>
              <div className="col-2">
                <div className="align-items-center">
                  <a
                    className="mr-2 fas fa-external-link-alt "
                    target="_blank"
                    rel="noopener noreferrer"
                    data-toggle="tooltip"
                    data-placement="top"
                    title={this.state.sponsor.companyUrl}
                    href={this.state.sponsor.companyUrl}
                  >
                    {""}
                  </a>
                </div>
              </div>
            </div>
            <p />
            <div className="row">
              <div className="col-1">
                <em className="icon-map" />
              </div>
              <div className="col">{this.state.sponsor.address}</div>
            </div>
            <div className="row">
              <div className="col-1">
                <em className="mr-2 fas fa-user-tie" />
              </div>
              <div className="col">{this.state.sponsor.contactPerson}</div>
            </div>
            <div className="row">
              <div className="col-1">
                <em className="mr-2 fas fa-phone-volume" />
              </div>
              <div className="col">
                <a href={"tel:" + this.state.sponsor.phoneNumber} style={phone}>
                  {this.formatPhoneNumber(this.state.sponsor.phoneNumber)}
                </a>
              </div>
            </div>
          </div>
          <CardFooter className="d-flex">
            <div>
              <button
                type="button"
                className="btn btn-sm btn-outline-primary"
                onClick={this.handleEdit}
              >
                Edit
              </button>
            </div>
            <div className="ml-auto">
              <button
                type="button"
                className="btn btn-sm  btn-outline-danger"
                onClick={this.handleDelete}
              >
                Delete
              </button>
            </div>
          </CardFooter>
        </div>
      </div>
    );
  }
}

export default CardSponsor;
