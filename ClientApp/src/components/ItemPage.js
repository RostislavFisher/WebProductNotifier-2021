import React, { Component } from 'react';
import './productPage.css';
import authService from "./api-authorization/AuthorizeService";
export class ItemPage extends React.Component {
    constructor() {
        super();
        authService.isAuthenticated().then(
            result =>  
                this.setState({
                    IsLoggined: result
            })
        )

        
        this.state = {
            personID: '',
            IsLoggined : false,
            IsInNotifyList: false,
            ItemID : '',
            Link : '',
            PriceUAH : '',
            PriceUSD : '',
            Title : '',
            description : '',
            img : '',
        };
        this.addToNotify = this.addToNotify.bind(this)
        this.deleteFromNotify = this.deleteFromNotify.bind(this)


    }
    componentDidMount() {
        this.search();
        if (this.state.IsLoggined === true) {
            const makeRequest = async () => {
                var userIDPromise = await authService.getUser()
                this.setState({personID: userIDPromise["sub"]})

                this.checkIfAlreadyAdded();
            }

            makeRequest();
            
        }
    }
    
    checkIfAlreadyAdded(){
        fetch(`https://localhost:5001/Notify/CheckIfAlreadyAdded?shopKey=${this.props.match.params.shopKey}&ItemID=${this.props.match.params.ItemID}&personID=${this.state.personID}`)
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({IsInNotifyList: (result.value==="True") });
                    console.log(this.state.IsInNotifyList)
                },
                (error) => {
                    this.setState({
                        isLoaded: true,
                        error
                    });
                }
            )

    }
    addToNotify(){
        fetch(`https://localhost:5001/Notify/AddToNotify?shopKey=${this.props.match.params.shopKey}&ItemID=${this.props.match.params.ItemID}&Price=${this.state.PriceUAH}&personID=${this.state.personID}`).then(res => res.json()).then(
            (result) => {

                this.checkIfAlreadyAdded();
            }
        );
    }    
    deleteFromNotify(){
        fetch(`https://localhost:5001/Notify/DeleteFromNotify?shopKey=${this.props.match.params.shopKey}&ItemID=${this.props.match.params.ItemID}&personID=${this.state.personID}`).then(res => res.json()).then(
            (result) => {

                this.checkIfAlreadyAdded();
            }
        );
    }
    search() {
        fetch(`https://localhost:5001/SearchProduct/SearchInShopByCode?shopKey=${this.props.match.params.shopKey}&ItemID=${this.props.match.params.ItemID}`)
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({ItemID: result.ItemID });
                    this.setState({Link: result.Link });
                    this.setState({PriceUAH: result.PriceUAH });
                    this.setState({PriceUSD: result.PriceUSD });
                    this.setState({Title: result.Title });
                    this.setState({description: result.description });
                    this.setState({img: result.img });
                },
                (error) => {
                    this.setState({
                        isLoaded: true,
                        error
                    });
                }
            )
    }
    render(){
        return(
            
            <div class="container-fluid">
                <div class="content-wrapper">
                    <div class="item-container">
                        <div class="container">
                            <div class="product col-md-3 service-image-left">
                                <center>
                                    <img id="item-display" src={this.state.img} alt=""></img>
                                </center>
                            </div>
                            <div class="col-md-7">
                                <div class="product-title">{this.state.Title}</div>
                                <div class="product-desc"><div dangerouslySetInnerHTML={{ __html: this.state.description }} /></div>
                                <div class="product-rating"><i class="fa fa-star gold"></i> <i class="fa fa-star gold"></i> <i class="fa fa-star gold"></i> <i class="fa fa-star gold"></i> <i class="fa fa-star-o"></i> </div>
                                <hr></hr>
                                    <div class="product-price">$ {this.state.PriceUSD}</div>
                                    <div class="product-price">₴ {this.state.PriceUAH}</div>
                                    <div class="product-stock">In Stock</div>
                                    <hr></hr>
                                        <div class="btn-group cart">
                                            <button type="button" class="btn btn-success" disabled={this.state.IsInNotifyList || !this.state.IsLoggined} onClick={this.addToNotify}>
                                                Add to wishlist
                                            </button>
                                            <button type="button" class="btn btn-danger" disabled={!this.state.IsInNotifyList || !this.state.IsLoggined} onClick={this.deleteFromNotify}>
                                                Delete from wishlist
                                            </button>
                                        </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}