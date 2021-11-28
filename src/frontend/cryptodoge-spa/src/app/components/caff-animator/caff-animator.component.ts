import { CiffDto } from './../../../../generated/client/model/ciffDto';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { CaffDto } from 'generated/client';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-caff-animator',
  templateUrl: './caff-animator.component.html',
  styleUrls: ['./caff-animator.component.scss']
})
export class CaffAnimatorComponent implements OnInit, OnDestroy {

  @Input('caff') caff: CaffDto;
  @Input('shouldStop') shouldStop: boolean;
  stopRequested: boolean;

  currentSrc: string;
  private currentIdx = 0;

  constructor() { 
    /* Empty */
   }

  ngOnInit() {
    this.looper(this.getNextCiff());
  }

  getNextCiff(): CiffDto {
    if (this.currentIdx >= this.caff.ciffs.length) {
      this.currentIdx = 0;
    }
    let c = this.caff.ciffs[this.currentIdx];
    this.currentIdx++;
    return c;
  }

  looper(ciff: CiffDto) {
    if (this.shouldStop || this.stopRequested) {
      return;
    }

    let interval = ciff.duration;
    this.currentSrc = `${environment.imagesBasePath + ciff.id}.png`;

    setTimeout(() => {
      let c = this.getNextCiff();
      this.looper(c);
    }, interval);
  }

  stopAnimation() {
    this.stopRequested = true;
  }

  ngOnDestroy() {
    this.stopRequested = true;
  }

}
