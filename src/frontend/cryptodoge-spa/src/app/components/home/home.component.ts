import { HttpClient, HttpResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { saveAs } from 'file-saver';
import { Configuration } from '../../../../generated/client/configuration';
import { CaffDto, ImagesService, SearchByCaptionDto, SearchByTagsDto } from 'generated/client';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  constructor(private imagesService: ImagesService, private http: HttpClient) { }

  caffs: CaffDto[];
  tags: string[] = [];
  captions: string[] = [];

  tagSearchValues: string[];
  captionSearchValue: string;

  filteredTags: string[] = [];
  filteredCaptions: string[] = [];

  ngOnInit(): void {
    this.imagesService.imagesGetCaffs().subscribe(res => {
      this.caffs = res
      this.caffs.forEach(caff => {
        this.tags = [...caff.tags];
        this.captions = [...caff.captions];
        this.filteredTags = this.tags;
        this.filteredCaptions = this.captions;
      });
    });
  }

  filterTags(event) {
    let searchString = (event.query ?? '').toLowerCase();
    if (searchString.length < 3) {
      return;
    }
    this.filteredTags = this.tags.filter(tag => tag.startsWith(searchString));
  }

  filterCaptions(event) {
    let searchString = (event.query ?? '');
    if (searchString.length < 3) {
      return;
    }
    this.filteredCaptions = this.captions.filter(tag => tag.startsWith(searchString));
  }

  download(id: string) {

    this.downloadFile(id).subscribe((response: HttpResponse<Blob>) => {
      let filename: string = `${id}.caff`;
      let binaryData = [];
      binaryData.push(response.body);
      let downloadLink = document.createElement('a');
      downloadLink.href = window.URL.createObjectURL(new Blob(binaryData, { type: 'blob' }));
      downloadLink.setAttribute('download', filename);
      document.body.appendChild(downloadLink);
      downloadLink.click();
    })
  }

  //Kinda hacky, but api generator doesn't work for this.
  downloadFile(caffid: string) {
    return this.http.get<Blob>(`http://localhost:5000/api/images/${caffid}/download`, { observe: 'response', responseType: 'blob' as 'json' })
  }

  search() {
    this.caffs = [];

    if (this.tagSearchValues) {
      let tagsQueryObject: SearchByTagsDto =
      {
        queryTags: this.tagSearchValues
      };
      this.imagesService.imagesSearchCaffsByTags(tagsQueryObject).subscribe(res => {
        this.caffs = this.caffs.concat(res)
        if (this.captionSearchValue) {
          let captionQueryObject: SearchByCaptionDto =
          {
            query: this.captionSearchValue
          };
          this.imagesService.imagesSearchCaffsByCaption(captionQueryObject).subscribe(res => {
            res.forEach(caff => {
              if (!this.caffs.some(c => c.id == caff.id)) {
                this.caffs.push(caff);
              }
            });
            if (this.tagSearchValues.length == 0 && this.captionSearchValue.length == 0) {
              this.imagesService.imagesGetCaffs().subscribe(res => this.caffs = res)
            }
          });
        }
      });
    }
    else {
      if (this.captionSearchValue) {
        let captionQueryObject: SearchByCaptionDto =
        {
          query: this.captionSearchValue
        };
        this.imagesService.imagesSearchCaffsByCaption(captionQueryObject).subscribe(res => {
          res.forEach(caff => {
            if (!this.caffs.some(c => c.id == caff.id)) {
              this.caffs.push(caff);
            }
          });
          if (this.tagSearchValues.length == 0 && this.captionSearchValue.length == 0) {
            this.imagesService.imagesGetCaffs().subscribe(res => this.caffs = res)
          }
        });
      }
    }

  }
}
