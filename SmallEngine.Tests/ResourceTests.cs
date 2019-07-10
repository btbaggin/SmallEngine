using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SmallEngine;
using SmallEngine.Graphics;

namespace Tests
{
    class ResourceTests
    {
        [Test]
        public void ResourceDisposal()
        {
            var bm = new BitmapResource();
            ResourceManager.Add("ResourceDisposal", bm);
            Assert.AreEqual(0, bm.ReferenceCount, "Incorrect initial count");

            ResourceManager.Request<BitmapResource>("ResourceDisposal");
            Assert.AreEqual(1, bm.ReferenceCount, "Incorrect reference count");

            bm.Dispose();

            Assert.AreEqual(0, bm.ReferenceCount, "Incorrect count after dispose");
            Assert.IsTrue(bm.Disposed);
        }

        [Test]
        public void ResourceDisposalHighReferenceCount()
        {
            var bm = new BitmapResource();
            ResourceManager.Add("ResourceDisposalHighReferenceCount", bm);

            ResourceManager.Request<BitmapResource>("ResourceDisposalHighReferenceCount");
            ResourceManager.Request<BitmapResource>("ResourceDisposalHighReferenceCount");

            bm.Dispose();
            Assert.IsFalse(bm.Disposed);

            bm.Dispose();
            Assert.IsTrue(bm.Disposed);
        }

        [Test]
        public void SubBitmaps()
        {
            var bm = new BitmapResource();
            ResourceManager.Add("SubBitmaps", bm);

            var sub = bm.CreateSubBitmap(new Rectangle(0, 0, 1, 1));
            Assert.AreEqual(0, bm.ReferenceCount, "Incorrect base reference count after sub bitmap");
            Assert.AreEqual(1, sub.ReferenceCount, "Incorrect subbitmap count");

            sub.Dispose();
            Assert.AreEqual(0, bm.ReferenceCount, "Incorrect base reference count after sub disposal");
            Assert.AreEqual(0, sub.ReferenceCount, "Incorrect sub bitmap count after disposal");
        }
    }
}
