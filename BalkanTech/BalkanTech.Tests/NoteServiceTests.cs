using BalkanTech.Data;
using BalkanTech.Services.Data;
using BalkanTech.Services.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Legacy;

namespace BalkanTech.Tests
{
    public class Tests
    {
        private BalkanDbContext context;
        private NoteService noteService;
        private Guid taskId;
        private Guid userId;
        private string noteComment;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BalkanDbContext>()
            .UseInMemoryDatabase(databaseName: "BalkanTech")
            .Options;
            this.context = new BalkanDbContext(options);
            this.noteService = new NoteService(context);
            taskId = Guid.NewGuid();
            userId = Guid.NewGuid();
            noteComment = "testing service";
        }
        [TearDown]
        public void TearDown()
        {
            context.Dispose();
        }

        [Test]
        public async Task AddNoteAsyncShouldBeSuccessfullWhenNoteAdded()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var noteComment = "testing service";
            await noteService.AddNoteAsync(noteComment, taskId, userId);
            var note = await context.Notes.FirstOrDefaultAsync(n => n.NoteComment == noteComment && n.MaintananceTaskId == taskId && n.AppUserId == userId);
            Assert.That(note, Is.Not.Null);
            Assert.That(note.NoteComment, Is.EqualTo(noteComment));
            Assert.That(note.MaintananceTaskId, Is.EqualTo(taskId));
            Assert.That(note.AppUserId, Is.EqualTo(userId));
        }
        [Test]
        public void NoteWithEmptyCommentShouldThrowException() 
        {
            noteComment = "";
            Assert.ThrowsAsync<ArgumentException>(() => noteService.AddNoteAsync(noteComment, taskId, userId));
        }
        [Test]
        public void NoteWithNullTaskIdShouldThrowException()
        {
            taskId = Guid.Empty;
            Assert.ThrowsAsync<ArgumentException>(() => noteService.AddNoteAsync(noteComment, taskId, userId));
        }
    }
}