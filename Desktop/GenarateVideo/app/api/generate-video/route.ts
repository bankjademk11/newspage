import { NextRequest, NextResponse } from 'next/server';

export async function POST(request: NextRequest) {
  try {
    const { prompt } = await request.json();

    if (!prompt) {
      return NextResponse.json(
        { error: 'Prompt is required' },
        { status: 400 }
      );
    }

    const apiKey = process.env.HUGGINGFACE_API_KEY;
    if (!apiKey) {
      return NextResponse.json(
        { error: 'API key not configured' },
        { status: 500 }
      );
    }

    console.log('🚀 Starting video generation with damo-vilab/text-to-video-ms-1.7b...');
    
    try {
      // Direct API call to Hugging Face with new endpoint
      const response = await fetch(
        `https://router.huggingface.co/hf-inference/models/damo-vilab/text-to-video-ms-1.7b`,
        {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${apiKey}`,
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            inputs: prompt,
            parameters: {
              num_frames: 16,
              num_inference_steps: 25,
              guidance_scale: 7.5
            }
          }),
        }
      );

      console.log('Response status:', response.status);
      console.log('Response headers:', Object.fromEntries(response.headers.entries()));

      if (!response.ok) {
        const errorText = await response.text();
        console.error('API Error:', errorText);
        
        // Check for credit limit error
        if (response.status === 402) {
          return NextResponse.json({
            error: 'API Credits Exhausted',
            message: 'You have exceeded your monthly included credits for Hugging Face Inference. Please upgrade to PRO or wait for credits to reset.',
            suggestion: 'Consider using a different API key or upgrading your Hugging Face account.',
            fallback: 'Generating a demo response instead.'
          }, { status: 402 }); // Return 402 for API credit exhaustion
        }
        
        throw new Error(`API Error: ${response.status} - ${errorText}`);
      }

      const contentType = response.headers.get('content-type');
      console.log('Response content type:', contentType);

      if (contentType?.includes('video')) {
        // Return video data directly
        const videoBuffer = await response.arrayBuffer();
        return new NextResponse(videoBuffer, {
          headers: {
            'Content-Type': 'video/mp4',
            'Content-Disposition': 'attachment; filename="generated-video.mp4"'
          },
        });
      } else if (contentType?.includes('image')) {
        // Return image data (fallback)
        const imageBuffer = await response.arrayBuffer();
        return new NextResponse(imageBuffer, {
          headers: {
            'Content-Type': 'image/jpeg',
            'Content-Disposition': 'attachment; filename="generated-image.jpg"'
          },
        });
      } else {
        // Handle JSON response
        const data = await response.json();
        console.log('JSON response:', data);
        
        return NextResponse.json({
          success: true,
          model: 'damo-vilab/text-to-video-ms-1.7b',
          data: data,
          note: "Video generation attempted with damo-vilab/text-to-video-ms-1.7b"
        });
      }

    } catch (videoError) {
      console.error('❌ Video generation failed:', videoError);
      
      // Check if it's a credit limit error
      if (videoError instanceof Error && videoError.message.includes('402')) {
        return NextResponse.json({
          error: 'API Credits Exhausted',
          message: 'You have exceeded your monthly included credits for Hugging Face Inference.',
          suggestion: 'Please upgrade your Hugging Face account or use a different API key.',
          demoMode: true
        }, { status: 402 }); // Return 402 for API credit exhaustion
      }
      
      // Fallback to image generation
      console.log('🔄 Falling back to image generation...');
      
      try {
        const imageResponse = await fetch(
          `https://router.huggingface.co/hf-inference/models/stabilityai/stable-diffusion-2-1`,
          {
            method: 'POST',
            headers: {
              'Authorization': `Bearer ${apiKey}`,
              'Content-Type': 'application/json',
            },
            body: JSON.stringify({
              inputs: prompt,
              parameters: {
                num_images: 1,
                guidance_scale: 7.5,
                num_inference_steps: 50
              }
            }),
          }
        );

        console.log('Image fallback response status:', imageResponse.status);

        if (!imageResponse.ok) {
          const errorText = await imageResponse.text();
          console.error('Image fallback error:', errorText);
          
          // Check for credit limit error
          if (imageResponse.status === 402) {
            return NextResponse.json({
              error: 'API Credits Exhausted',
              message: 'You have exceeded your monthly included credits for Hugging Face Inference.',
              suggestion: 'Please upgrade your Hugging Face account or use a different API key.',
              demoMode: true
            }, { status: 402 }); // Return 402 for API credit exhaustion
          }
          
          throw new Error(`Image fallback failed: ${imageResponse.status} - ${errorText}`);
        }

        const imageBuffer = await imageResponse.arrayBuffer();
        console.log('✅ Image generation successful as fallback');

        return new NextResponse(imageBuffer, {
          headers: {
            'Content-Type': 'image/jpeg',
            'Content-Disposition': 'attachment; filename="generated-image.jpg"'
          },
        });

      } catch (imageError) {
        console.error('❌ Image generation also failed:', imageError);
        
        // Check if it's a credit limit error
        if (imageError instanceof Error && imageError.message.includes('402')) {
          return NextResponse.json({
            error: 'API Credits Exhausted',
            message: 'You have exceeded your monthly included credits for Hugging Face Inference.',
            suggestion: 'Please upgrade your Hugging Face account or use a different API key.',
            demoMode: true
          }, { status: 402 }); // Return 402 for API credit exhaustion
        }
        
        return NextResponse.json(
          { 
            error: 'Both video and image generation failed',
            videoError: videoError instanceof Error ? videoError.message : 'Unknown video error',
            imageError: imageError instanceof Error ? imageError.message : 'Unknown image error'
          },
          { status: 500 }
        );
      }
    }

  } catch (error) {
    console.error('Generation error:', error);
    return NextResponse.json(
      { 
        error: 'Generation failed', 
        details: error instanceof Error ? error.message : 'Unknown error'
      },
      { status: 500 }
    );
  }
}