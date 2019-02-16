import React from "react";
import icons from "./SponsorsConstatns";

class BlockSponsor extends React.Component {
  constructor(props) {
    super(props);
    if (this.props.data !== undefined) {
      this.state = {
        type: { id: this.props.data.id, name: this.props.data.name }
      };
    } else {
      this.state = { type: { id: 0, name: "All Sponsors" } }; //id = 0 means all sponsors
    }
  }
  headleShowSponsors = () => {
    if (this.state.type.id === 0) {
      this.props.history.push({
        pathname: "/sponsors/all",
        state: {
          icon: this.props.icon
        }
      });
    } else {
      this.props.history.push({
        pathname: "/sponsors/category/" + this.state.type.id,
        state: {
          icon: this.props.icon
        }
      });
    }
  };
  render() {
    return (
      <div className="col-lg-4">
        <div className="card b">
          <div className="card-body text-center">
            <a
              className="link-unstyled text-info"
              onClick={this.headleShowSponsors}
            >
              <em className={"fa-5x mb-3 " + icons.ic[this.state.type.id]} />
              <br />
              <span className="h4">{this.state.type.name}</span>
              <br />
              <div className="text-sm text-muted">View all →</div>
            </a>
          </div>
        </div>
      </div>
    );
  }
}

export default BlockSponsor;
