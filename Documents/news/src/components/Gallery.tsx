import React from 'react'

const images = Array.from({length:8}).map((_,i)=>`/assets/placeholder-${i+1}.svg`)

export default function Gallery(){
  return (
    <section>
      <h3 className="text-xl font-semibold mb-4">Work gallery</h3>
      <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
        {images.map((src, idx)=>(
          <div key={idx} className="bg-white rounded-lg shadow p-2">
            <img src={src} alt={`placeholder ${idx+1}`} className="w-full h-40 object-cover rounded" />
            <div className="mt-2 text-sm text-slate-600">Caption {idx+1}</div>
          </div>
        ))}
      </div>
    </section>
  )
}
