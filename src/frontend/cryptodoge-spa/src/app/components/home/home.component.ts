import { ToastService } from 'src/app/services/toast.service';
import { UserService } from 'src/app/services/user.service';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { CaffDto, ImagesService, SearchByCaptionDto, SearchByTagsDto } from 'generated/client';
import { ConfirmationService } from 'primeng/api';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  constructor(private toaster: ToastService, private confirmationService: ConfirmationService,
    private imagesService: ImagesService, private userService: UserService, private http: HttpClient) { }

  isLoggedIn: Boolean;
  private subscribtion: Subscription;

  caffs: CaffDto[];
  tags: string[] = [];
  captions: string[] = [];

  tagSearchValues: string[];
  captionSearchValue: string;

  filteredTags: string[] = [];
  filteredCaptions: string[] = [];

  ngOnInit(): void {
    this.subscribtion = this.userService.$isLoggedIn.subscribe(data => {
      this.isLoggedIn = data
    });

    if (this.isLoggedIn) {
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
      this.imagesService.imagesSearchCaffsByTags(tagsQueryObject).subscribe(caffsByTags => {
        this.caffs = this.caffs.concat(caffsByTags)
        if (this.captionSearchValue) {
          let captionQueryObject: SearchByCaptionDto =
          {
            query: this.captionSearchValue
          };
          this.imagesService.imagesSearchCaffsByCaption(captionQueryObject).subscribe(caffsByCaption => {
            this.updateCaffsByCaption(caffsByCaption);
            if (this.tagSearchValues.length == 0 && this.captionSearchValue.length == 0) {
              this.imagesService.imagesGetCaffs().subscribe(allCaffs => this.caffs = allCaffs)
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
          this.updateCaffsByCaption(res);
          if (this.tagSearchValues.length == 0 && this.captionSearchValue.length == 0) {
            this.imagesService.imagesGetCaffs().subscribe(allCaffs => this.caffs = allCaffs)
          }
        });
      }
    }
  }

  updateCaffsByCaption(caffResult: CaffDto[]) {
    caffResult.forEach(caff => {
      if (!this.caffs.some(c => c.id == caff.id)) {
        this.caffs.push(caff);
      }
    });
  }

  isAdmin() {
    return this.userService.hasRole(["ADMIN"]);
  }

  deleteCaff(event, id: string) {
    this.confirmationService.confirm({
      target: event.target,
      message: 'Are you sure that you want to delete this CAFF?',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.imagesService.imagesDeleteCaff(id).subscribe(() => {
          this.toaster.success("CAFF deleted");
          this.caffs = this.caffs.filter(x => x.id !== id);
        }, err => {
          this.toaster.error("Error during CAFF deletion");
        });
      },
      reject: () => {
        //reject action
      }
    });
  }

}
