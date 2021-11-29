import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.scss']
})
export class DetailsComponent implements OnInit {

  id:String;
  constructor(private route: ActivatedRoute) {
    this.id = this.route.snapshot.params.id;
   }

  ngOnInit(): void {
  }

}
