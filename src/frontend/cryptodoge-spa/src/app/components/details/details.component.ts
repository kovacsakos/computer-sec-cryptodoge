import { ToastService } from 'src/app/services/toast.service';
import { CaffDto } from './../../../../generated/client/model/caffDto';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ImagesService } from 'generated/client';
import { HttpResponse, HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.scss']
})
export class DetailsComponent implements OnInit {

  id: string;
  caff: CaffDto;
  newComment: string;
  @ViewChild('op') overLay;
  
  constructor(
    private toaster: ToastService,
    private route: ActivatedRoute, private imagesService: ImagesService, private http: HttpClient) {
    this.id = this.route.snapshot.params.id;
  }

  ngOnInit(): void {
    this.imagesService.imagesGetCaffByIdAsync(this.id).subscribe(result => {
      this.caff = result;
    });
  }

  sendComment() {
    this.imagesService.imagesPostComment(this.caff.id, {comment: this.newComment}).subscribe(newCommentId => {
      this.overLay.hide();
      this.newComment = '';
      this.toaster.success("Comment added");
      this.imagesService.imagesGetComment(newCommentId).subscribe(addedComment => {
        this.caff.comments.unshift(addedComment);
      });   
    }, err => {
      this.toaster.error("Error, comment was not added.")
    });
  }

  commentShouldBeDeleted(commentId: string) {
    this.imagesService.imagesDeleteComment(commentId).subscribe(res => {
      this.toaster.success("Comment deleted");
      this.caff.comments = this.caff.comments.filter(x => x.id !== commentId);
    }, err => {
      this.toaster.error("Comment deletion failed");
    });
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

}
