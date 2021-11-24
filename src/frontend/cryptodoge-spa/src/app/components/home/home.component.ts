import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  constructor() { }

  tags: string[]
  searchValue: string;

  ngOnInit(): void {
    //TODO Get tags
    //TODO Get Latest CAFFS
  }

  search(event){
    //TODO Get CAFFS by tags
  }
}
