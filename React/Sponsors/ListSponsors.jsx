import React from "react";
import * as sponsorServices from "../../services/sponsorServices";
import ContentWrapper from "../Layout/ContentWrapper";
import CardSponsor from "../../components/Sponsors/CardSponsor";
import Pagination from "react-js-pagination";
import Swipe from "react-easy-swipe";

class ListSponsors extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      sponsors: [],
      activePage: 0,
      totalSponsorsCount: null,
      sponsorType: null,
      itemsCountPerPage: 4
    };
    this.renderBlockSponsor = this.renderBlockSponsor.bind(this);
  }

  handlePageChange = pageNumber => {
    this.GetSponsors(
      pageNumber,
      this.state.itemsCountPerPage,
      this.props.match.params.categoryId
    );
  };
  GetSponsors(pageNumber, pageSize, categoryId) {
    sponsorServices
      .getByPageIndexPageSizeType(pageNumber - 1, pageSize, categoryId) //get by page
      .then(this.onGetSponsorsSucsses)
      .catch(this.onGetSponsorsError);
  }

  componentDidMount() {
    this.GetSponsors(
      1,
      this.state.itemsCountPerPage,
      this.props.match.params.categoryId
    );
  }
  onGetSponsorsSucsses = response => {
    this.setState({
      activePage: response.item.pageIndex,
      sponsors: response.item.pagedItems,
      totalSponsorsCount: response.item.totalCount,
      sponsorType:
        this.props.match.params.categoryId === undefined
          ? "All Sponsors"
          : "Sponsors By " + response.item.pagedItems[0].sponsorType.name
    });
  };
  onGetSponsorsError = response => {
    this.setState({
      sponsorType: "There are no Sponsors"
    });
  };

  renderBlockSponsor = sponsor => {
    return <CardSponsor {...this.props} key={sponsor.id} data={sponsor} />;
  };

  onSwipeMove(position, event, props) {
    if (position.x > 145 && position.y < 50 && position.y > -50) {
      props.history.push({
        pathname: "/sponsors/display"
      });
    }
  }

  render() {
    return (
      <ContentWrapper>
        <Swipe
          onSwipeMove={(position, event) =>
            this.onSwipeMove(position, event, this.props)
          }
        >
          <div className="container-fluid pt-3">
            <div className="text-center mb-3 pb-3 ">
              <div className="h2 text-bold">{this.state.sponsorType}</div>
            </div>
            <div className="d-flex justify-content-center">
              {this.state.sponsorType !== "There are no Sponsors" ? (
                <Pagination
                  activePage={this.state.activePage + 1}
                  itemsCountPerPage={this.state.itemsCountPerPage}
                  totalItemsCount={this.state.totalSponsorsCount}
                  pageRangeDisplayed={3}
                  onChange={this.handlePageChange}
                  itemClass="page-item"
                  linkClass="page-link"
                />
              ) : (
                ""
              )}
            </div>
            <div className="row">
              {this.state.sponsors.map(this.renderBlockSponsor)}
            </div>
          </div>
        </Swipe>
      </ContentWrapper>
    );
  }
}

export default ListSponsors;
