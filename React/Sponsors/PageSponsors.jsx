import React from "react";
import ContentWrapper from "../Layout/ContentWrapper";
import BlockSponsor from "./BlockSponsor";
import * as sponsorServices from "../../services/sponsorServices";

class PageSponsors extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      sponsorTypes: {
        list: []
      },
      ic: {
        0: "far fa-address-book",
        1: "fab fa-gulp",
        2: "fas fa-store",
        3: "fas fa-utensils",
        4: "fas fa-coffee",
        5: "fas fa-basketball-ball",
        6: "fas fa-child"
      }
    };
  }
  componentDidMount() {
    sponsorServices
      .getTypes()
      .then(this.onGetTypesSucsses)
      .catch(this.onGetTypesError);
  }
  onGetTypesSucsses = response => {
    this.setState({
      sponsorTypes: { list: response.items }
    });
  };
  onGetTypesError = response => {
    console.log(response);
  };

  renderBlockSponsor = sponsor => {
    return (
      <BlockSponsor
        {...this.props}
        key={sponsor.id}
        data={sponsor}
        icon={this.state.ic[sponsor.id]}
      />
    );
  };
  render() {
    return (
      <ContentWrapper>
        <div className="unwrap">
          <div className="bg-cover">
            <div className="container container-md py-4">
              <div className="text-center mb-3 pb-3">
                <div className="h1 text-bold">Sponsors</div>
                <p>Here you can find our Sponsors</p>
              </div>
            </div>
          </div>
        </div>
        <div className="container container-md">
          <div className="row">
            <BlockSponsor {...this.props} icon={this.state.ic[0]} />
            {this.state.sponsorTypes.list.map(this.renderBlockSponsor)}
          </div>
        </div>
      </ContentWrapper>
    );
  }
}

export default PageSponsors;
