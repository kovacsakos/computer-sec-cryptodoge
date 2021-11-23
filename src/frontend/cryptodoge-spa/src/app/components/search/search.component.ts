import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent implements OnInit {

  constructor() { }

  tags: string[];
  searchValue: string;


  ngOnInit(): void {
    //TODO get tags
  }

  search(event){
    //TODO get CAFFS
  }
}
