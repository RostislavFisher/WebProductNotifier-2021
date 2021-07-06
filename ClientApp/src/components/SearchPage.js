import React, { Component } from 'react';

import {leftMenu} from "../card.css";
import {TextField} from "@material-ui/core";

export class SearchPage extends React.Component {
    constructor () {
        class Card {
            constructor(Title, Link, img, Country, PriceUAH, PriceUSD, ItemID ){
                this.Title = Title;
                this.Link = Link;
                this.img = img;
                this.Country = Country;
                this.PriceUAH = PriceUAH;
                this.PriceUSD = PriceUSD;
                this.ItemID = ItemID
            }
        }
        super();
        this.state = {
            Title : '',
            ShopTitle: '',
            PriceScopeFirst: 0,
            PriceScopeSecond: 0,
            Manufacturer: '',
            cardResult: [new Card("test", "test", "test", "test", "test", "test", "test")],
            resultSearch: '',
        };
        this.handleChange = this.handleChange.bind(this);
        this.search = this.search.bind(this);
        

    }
        search() {
            fetch(`https://localhost:5001/SearchProduct/Search?ShopTitle=${this.state.ShopTitle}&Manufacturer=${this.state.Manufacturer}&FirstPriceScope=${this.state.PriceScopeFirst}&SecondPriceScope=${this.state.PriceScopeSecond}&Title=${this.state.Title}/`)
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({cardResult: result })
                    this.setState({resultSearch:
                            this.state.cardResult.map(function(objectSearch) {
                                console.log("helooo");
                                    return(
                                        <div class="card">
                                            <img src={`${objectSearch.img}`} class="card-img-top" alt="..."/>
                                            <div class="card-body">
                                                <h5 class="card-title">{`${objectSearch.Title}`}</h5>
                                                <p class="card-text">{`${objectSearch.PriceUAH}`}₴</p>
                                                <a href={`ItemPage/${objectSearch["ShopObject"].ShopTitle}/${objectSearch.ItemID}`} class="btn btn-primary">Перейти</a>
                                            </div>
                                        </div>
                                    );
                                }
                            )
                    });
                },
                (error) => {
                    this.setState({
                        isLoaded: true,
                        error
                    });
                }
            )
    }
    handleChange (evt, field) {
        this.setState({ [field]: evt.target.value });
    }

    render () {
        
        return (
            <div>
                <div class="row">
                        <form>
                            <p>
                                <TextField onChange={(event)=>this.handleChange(event, "Title")} label="Название товара" variant="outlined" />
                            </p>
                            <p>
                                <TextField onChange={(event)=>this.handleChange(event, "ShopTitle")} label="Название магазина" variant="outlined" /></p>
                            <p>
                                <p>
                                    <TextField type="number" min="1" onChange={(event)=>this.handleChange(event, "PriceScopeFirst")} label="Цена от" variant="outlined" />
                                </p>
                                <TextField type="number" min="1" onChange={(event)=>this.handleChange(event, "PriceScopeSecond")} label="Цена до" variant="outlined" />
                            </p>
                            <p>
                                <TextField onChange={(event)=>this.handleChange(event, "Manufacturer")} label="Производитель" variant="outlined"/>
                            </p>
                            <input type="button" value="Отправить" onClick={this.search}/>
                        </form>
                <div class="col-md-8">
                    {this.state.resultSearch}
                </div>
                </div>

            </div>
        );
    }
}
